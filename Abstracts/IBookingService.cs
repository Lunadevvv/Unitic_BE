using Unitic_BE.DTOs;
using Unitic_BE.Entities;

public interface IBookingService
{
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetAllUserBooking(string userId);
    Task<Booking?> GetByIdAsync(string bookingId);
    Task BuyTicketAsync(BookingRequest request, string userId);
    Task TransferTicketAsync(string bookingId, string userId, string userIdTransfer);
}