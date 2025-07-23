using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Unitic_BE.Enums;

namespace Unitic_BE.Entities
{
    public class Event
    {
        [Key]
        public string EventID { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date_Start { get; set; }
        public DateTime Date_End { get; set; }
        public int Price { get; set; } = 0;
        public EventStatus Status { get; set; } = EventStatus.Private;

        public string CateID { get; set; } = string.Empty;
        public int Slot { get; set; } = 0;
        [JsonIgnore] // Prevent circular reference during serialization

        [ForeignKey("CateID")]
        public Category Category { get; set; } // Navigation property to the Category entity
        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        [JsonIgnore]
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
