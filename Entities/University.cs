using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Unitic_BE.Entities
{
    public class University
    {
        [Key]
        public required string Id { get; set; } // Unique identifier for the university, can be a GUID or any other unique string.
        public required string Name { get; set; } // Name of the university, used for display purposes.
        [JsonIgnore]
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

