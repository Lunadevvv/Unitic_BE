using System.ComponentModel.DataAnnotations;

namespace Unitic_BE.Entities;

public class Feedback
{
    [Key]
    public required string FeedbackId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}