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

        // [HttpGet("VnPayUrl")]
        // public IActionResult GetVnPayUrl()
        // {

        // }
        [HttpPost("checkout")]
        public IActionResult CheckoutByVnPay(int accId)
        {
            // var account = _context.AccountTbls.FirstOrDefault(acc => acc.AccId.Equals(accId));
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
                Description = "Thank you for purchasing our Membership",
                FullName = name
            };
            var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel, accId);
            return Ok(paymentUrl);
        }

        
         [HttpGet("vnpay-response")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if(response != null)
            {
                if(response.Success && response.VnPayResponseCode == "00")
                {                  
                    return Redirect("https://localhost:7163/Unitic/Payment/vnpay-response");
                }
            }
            return Redirect("https://localhost:7163/Unitic/Payment/vnpay-response");
        }
    }
}

