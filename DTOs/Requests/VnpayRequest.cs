using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Requests
{
    public class VnPaymentRequest
    {
        public int OrderId { get; set; }
        public required string FullName { get; set; }
        public required string Description { get; set; }
        public required double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }   
}