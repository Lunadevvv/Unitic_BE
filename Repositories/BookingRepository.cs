using Microsoft.EntityFrameworkCore;
using Unitic_BE;
using Unitic_BE.Entities;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings.ToListAsync();
    }
    public async Task<List<Booking>> GetAllUserBooking(string userId)
    {
        return await _context.Bookings
        .Where(b => b.AccountId == userId)
        .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(string bookingId)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task CreateAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        booking.UpdateDate = DateTime.UtcNow;
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string bookingId)
    {
        var booking = await GetByIdAsync(bookingId);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Booking?> GetLastBooking()
    {
        return await _context.Bookings
            .OrderByDescending(b => b.BookingId)
            .FirstOrDefaultAsync();
    }

}