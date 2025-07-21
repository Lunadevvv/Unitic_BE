using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.Entities
{
    public class Payment
    {
        [Key]
        public string PaymentId { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime PaidDate { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; } = string.Empty;

    }
}