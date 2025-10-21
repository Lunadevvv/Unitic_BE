namespace Unitic_BE.DTOs;

public record BookingRequest
{
    public required string EventID { get; set; }
    public required int Quantity { get; set; }

}