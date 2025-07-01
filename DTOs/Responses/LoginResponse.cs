using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Responses
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public string Token { get; set; }
    }
}
