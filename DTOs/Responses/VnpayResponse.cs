using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Responses
{
    public class VnPaymentResponse
    {
        public string? PaymentId { get; set; }
        public bool Success { get; set; } = false;
        public string? Money { get; set; }
        public string? PaymentDescription { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentTime { get; set; }
        public string? VnpayTransactionId { get; set; }
        public string? VnPayResponseCode { get; set; }
        public string? VnPayTransactionStatus { get; set; }
    } 
}