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
        public string CreatePaymentUrl(VnPaymentRequest model)
        {
            var tick = DateTime.Now.Ticks;
            var vnpay = new VnPayLibrary();
            string returnUrl = $"{_configuration["Vnpay:PaymentBackUrl"]}";
            vnpay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); 
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", model.IpAddress);
            vnpay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Pay for the ticket:" + model.PaymentId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", tick.ToString());
            var paymentUrl = vnpay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;
        }


        public VnPaymentResponse GetPaymentResult(IQueryCollection collections)
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
            var paymentId = vnpay.GetResponseData("vnp_TxnRef");
            var vnpayTranId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_CardType = vnpay.GetResponseData("vnp_CardType");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");
            var vnp_TransferTime = vnpay.GetResponseData("vnp_PayDate");
            var vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
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
                PaymentId = paymentId,
                Money = vnp_Amount,
                PaymentTime = vnp_TransferTime,
                Success = true,
                PaymentMethod = vnp_CardType,
                PaymentDescription= vnp_OrderInfo,
                VnpayTransactionId = vnpayTranId.ToString(),
                VnPayResponseCode = vnp_ResponseCode,
                VnPayTransactionStatus = vnp_TransactionStatus,
            };
        }
    }
}