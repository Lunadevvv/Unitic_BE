using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.DTOs.Requests
{
    public class UpdateUserInformation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mssv { get; set; }
        public string UniversityId { get; set; }
    }
}