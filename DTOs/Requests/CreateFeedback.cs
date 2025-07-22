
namespace Unitic_BE.DTOs.Requests;

public record CreateFeedback
{
    public required string EventId { get; init; }
    public required string BookingId { get; init; }
    public required string Review { get; init; }
}