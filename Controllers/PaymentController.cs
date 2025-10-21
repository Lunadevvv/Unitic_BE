using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;
using Unitic_BE.Services;
using Unitic_BE.Utilities;
using Unitic_BE.Enums;
using System.Security.Claims;
using Unitic_BE.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentService _paymentService;
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;
        public PaymentController(IVnPayService vnPayService, IPaymentService paymentService, IAccountService accountService, UserManager<User> userManager)
        {
            this._vnPayService = vnPayService;
            this._paymentService = paymentService;
            this._accountService = accountService;
            this._userManager = userManager;
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<ActionResult<List<Payment>>> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPayment();
            return Ok(payments);
        }

        [HttpGet("allUserPayment/{userId}")]
        [Authorize]
        public async Task<ActionResult<List<Payment>>> GetAllUserPayments(string userId)
        {
            var payments = await _paymentService.GetAllUserPayment(userId);
            return Ok(payments);
        }

        [HttpGet("{paymentId}")]
        [Authorize]
        public async Task<ActionResult<Payment>> GetPayment(string paymentId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) {
                return BadRequest("Invalid user");
            }
            var payment = await _paymentService.GetPayment(paymentId, userId);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpPost("pay")]
        [Authorize]
        public async Task<IActionResult> Pay([FromBody] PaymentRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found");
            await _paymentService.PayMoney(dto, userId);
            return Ok(new { Message = "Payment successful" });
        }
    

        /// <summary>
        /// Initializes a new payment request with the specified amount and description.
        /// </summary>
        /// <param name="moneyToPay">The amount of money to be paid, in smallest currency unit (e.g. VND).</param>
        /// <param name="description">The description or note associated with the payment.</param>
        /// <returns>A URL or token to redirect the user to the third-party payment gateway.</returns>
        [HttpGet("vnpay-request")]
        [Authorize]
        public async Task<IActionResult> CreateVnpayPayment([FromQuery] int money, [FromQuery] string description)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) {
                    return BadRequest("Invalid user");
                }
                var paymentId = await _paymentService.GeneratePaymentId();
                var ipAddress = Utils.GetIpAddress(HttpContext);
                var vnPayModel = new VnPaymentRequest
                {
                    PaymentId = paymentId, //Mã tham chiếu giao dịch (Transaction Reference). Đây là mã số duy nhất dùng để xác định giao dịch. Bắt buộc và không được trùng lặp giữa các giao dịch.
                    Amount = money,// Số tiền thanh toán. Không chứa ký tự phân cách thập phân, phần nghìn, hoặc ký hiệu tiền tệ.
                    CreatedDate = DateTime.Now,
                    Description = description, //Thông tin mô tả nội dung thanh toán, không dấu và không chứa ký tự đặc biệt.
                    IpAddress = ipAddress,//Địa chỉ IP của người thực hiện giao dịch.
                };
                var paymentModel = new Payment
                {
                    PaymentId = paymentId,
                    Price = money,
                    Status = PaymentStatus.InProgress.ToString()
                };
                await _paymentService.CreatePayment(paymentModel, userId);
                var paymentUrl = _vnPayService.CreatePaymentUrl(vnPayModel);
                return Ok(paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Function to get the payment callback, for now it will also get database. Until have a host server
        /// Show value to the client
        /// </summary>
        /// <returns></returns>
        [HttpGet("vnpay-response")]
        public async Task<IActionResult> PaymentCallBack()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnPayService.GetPaymentResult(Request.Query);
                    var userId = await _paymentService.GetUserByPaymentId(paymentResult.PaymentId);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return NotFound("User not found for the given payment ID.");
                    }
                    var user = await _userManager.FindByIdAsync(userId);
                    var payment = new Payment();
                    if (paymentResult.Success)
                    {
                        payment.PaymentId = paymentResult.PaymentId;
                        payment.Status = PaymentStatus.Success.ToString();
                        payment.Price = Int32.Parse(paymentResult.Money) / 100;
                        await _paymentService.UpdatePaymentStatus(payment);
                        var updateResult = await _accountService.UpdateUserWallet(user, payment.Price);
                        return Redirect("http://localhost:5173/wallet/payment/success");
                    }
                    else
                    {
                        payment.PaymentId = paymentResult.PaymentId;
                        payment.Status = PaymentStatus.Failed.ToString();
                        await _paymentService.UpdatePaymentStatus(payment);
                        return Redirect("http://localhost:5173/wallet/payment/failure");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
                
    }
}

