using System.Security.Claims;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.Contracts;
using Unitic_BE.Entities;
using Unitic_BE.Requests;
using Unitic_BE.Services;
using WebTicket.Domain.Requests;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("vnpay-request")]
        public IActionResult CheckoutByVnPay(int accId)
        {
            // var account = _context.AccountTbls.FirstOrDefault(acc => acc.AccId.Equals(accId));

            // Tạm thời fix cứng tên để sau sẽ lấy giá trị đồ trong db
            string name = "test";
            if(name == null)
            {
                name = "Customer";
            }
            var vnPayModel = new VnPaymentRequestModel
            {
                OrderId = new Random().Next(1000, 10000),
                Amount = 99000,
                CreatedDate = DateTime.Now,
                Description = "Thank you for purchasing",
                FullName = name
            };
            var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel, accId);
            return Ok(paymentUrl);
        }

        
         [HttpGet("vnpay-response")]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            // Response Code != 00 la ko thanh cong
            if (response == null || response.VnPayResponseCode != "00")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}

