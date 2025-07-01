using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Requests
{
    public class ForgotPasswordRequest
    {
        public required string Email { get; set; } = string.Empty;
    }
}