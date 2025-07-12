using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unitic_BE.Entities;

public class Booking
{
    [Key]
    public required string BookingId { get; set; } 
    public required string EventId { get; set; } 
    public string FeedbackId { get; set; } = string.Empty;
    public required string AccountId { get; set; } 
    public DateTime CreatedDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string Status { get; set; } = string.Empty;

}