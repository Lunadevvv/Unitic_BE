using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Requests
{
    public class FeedbackRequest
    {
        public required string BookingId { get; set; }
        public required string Content { get; set; } = string.Empty;
        public required string EventId { get; set; } = string.Empty;
    }
}