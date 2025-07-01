using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unitic_BE.Entities;

namespace Unitic_BE.DTOs.Responses
{
    public class ProfileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Mssv { get; set; } = string.Empty;
        public int Wallet { get; set; }
        public University? University { get; set; }

    }
}