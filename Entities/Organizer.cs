using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Unitic_BE.Entities
{
    public class Organizer
    {
        [Key]
        public string OrganizerId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string EventID { get; set; } = string.Empty;
        [ForeignKey("EventID")]
        public Event Event { get; set; }
    }
}