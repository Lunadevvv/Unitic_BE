using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unitic_BE.Entities;

public class Feedback
{
    [Key]
    public string FeedbackId { get; set; } = string.Empty;
    public required string BookingId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    [ForeignKey("BookingId")]
    public Booking Booking { get; set; }
    public string EventID { get; set; } = string.Empty;
    [ForeignKey("EventID")]
    public Event Event { get; set; }
}