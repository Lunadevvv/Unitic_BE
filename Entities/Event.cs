using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitic_BE.Entities
{
    public class Event
    {
        [Key]
        public string EventID { get; set; }

        public string name { get; set; }
        public string description { get; set; }
        public DateTime Date_Start { get; set; }
        public DateTime Date_End { get; set; }
        public int price { get; set; }
        public string status { get; set; }

        public string? CateID { get; set; }

        [ForeignKey("CateID")]
        public Category? Category { get; set; } // Navigation property to the Category entity

    }
}
