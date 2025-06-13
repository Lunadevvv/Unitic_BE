using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.Dtos
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Hobby { get; set; }
    }
}
