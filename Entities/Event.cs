using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Unitic_BE.Entities
{
    public class Event
    {
        [Key]
        public string? EventID { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date_Start { get; set; }
        public DateTime? Date_End { get; set; }
        public int? Price { get; set; }
        public string? Status { get; set; }

        public string? CateID { get; set; }
        public int? Slot { get; set; }
        [JsonIgnore] // Prevent circular reference during serialization

        [ForeignKey("CateID")]
        public Category? Category { get; set; } // Navigation property to the Category entity

    }
}
