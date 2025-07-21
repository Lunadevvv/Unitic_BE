using System.ComponentModel.DataAnnotations;

namespace Unitic_BE.Entities;

public class Feedback
{
    [Key]
    public string FeedbackId { get; set; } = string.Empty;
    public required string UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}