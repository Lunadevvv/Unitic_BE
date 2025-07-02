using System.Diagnostics;
using Microsoft.Identity.Client;
using Unitic_BE.Abstracts;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.DTOs.Responses;
using Unitic_BE.Entities;
using Unitic_BE.Utilities;

namespace Unitic_BE.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequest model, int accId)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            string returnUrl = $"{_configuration["Vnpay:PaymentBackUrl"]}";
            vnpay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); 
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            vnpay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Pay for the order:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            var paymentUrl = vnpay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentResponse PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }
            // Console.WriteLine("Lấy giá trị bên Response data");
            // foreach (KeyValuePair<string, string> pair in vnpay._responseData)
            // {
            //     Console.WriteLine(pair.Key + ": " + pair.Value);
            // }

            // Debugger.Break();
            var orderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnpayTranId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["Vnpay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponse
                {
                    Success = false,
                };
            }
            return new VnPaymentResponse
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = orderId.ToString(),
                TransactionId = vnpayTranId.ToString(),
                Token = vnp_SecureHash.ToString(),
                VnPayResponseCode = vnp_ResponseCode,
            };
        }
    }
}