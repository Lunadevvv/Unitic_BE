using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Requests
{
    public class UpdateUserInformation
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Mssv { get; set; }
        public required string UniversityId { get; set; }
    }
}