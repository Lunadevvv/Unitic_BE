using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTicket.Domain.Requests
{
    public class LoginResponse
    {
        public string Message { get; set; }
        public string Token { get; set; }
    }
}
