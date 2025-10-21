using Unitic_BE.Entities;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetAllUserBooking (string userId);
    Task<Booking?> GetByIdAsync(string bookingId);
    Task<Booking?> GetLastBooking();
    Task CreateAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(string bookingId);
}