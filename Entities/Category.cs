using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Unitic_BE.Entities
{
    public class Category
    {
        [Key]
        public string CateID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Is_Disable { get; set; } = false;
        [JsonIgnore]
        public ICollection<Event> Events { get; set; } = new List<Event>(); // Navigation property to the Event entity
    }
}
