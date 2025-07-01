using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Requests
{
    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}