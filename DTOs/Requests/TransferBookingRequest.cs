namespace Unitic_BE.DTOs.Requests;

public record TransferBookingRequest
{
    public required string BookingId { get; set; }
    public required string UserId { get; set; }
    public required string TransferAccountId { get; set; }
}