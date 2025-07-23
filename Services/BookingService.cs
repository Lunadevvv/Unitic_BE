using Unitic_BE.Abstracts;
using Unitic_BE.DTOs;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.Exceptions;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventService _eventService;
    private readonly IAccountService _accountService;

    public BookingService(IBookingRepository bookingRepository, IEventService eventService, IAccountService accountService)
    {
        _bookingRepository = bookingRepository;
        _eventService = eventService;
        _accountService = accountService;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _bookingRepository.GetAllAsync();
    }

    public async Task<Booking?> GetByIdAsync(string id)
    {
        return await _bookingRepository.GetByIdAsync(id);
    }

    public async Task<string> GenerateBookingId()
    {
        var lastBooking = await _bookingRepository.GetLastBooking();
        if (lastBooking == null) return "Booking0001";
        int id = int.Parse(lastBooking.BookingId.Substring(lastBooking.BookingId.Length - 4)) + 1; // lấy id cuối cùng và cộng thêm 1
        string generatedId = "Booking" + id.ToString("D4");
        return generatedId;
    }

    public async Task<List<Booking>> GetAllUserBooking(string userId)
    {
        return await _bookingRepository.GetAllUserBooking(userId);
    }

    public async Task BuyTicketAsync(BookingRequest request, string userId)
    {
        try
        {
            var evt = await _eventService.CheckEventStatusAsync(request.EventID)
                    ?? throw new ObjectNotFoundException($"{request.EventID}");
            if (evt.Status == EventStatus.InProgress || evt.Status == EventStatus.Completed)
                throw new Exception("Event already started or finished");
            if (evt.Status == EventStatus.SoldOut)
                throw new Exception("Event already sold out");
            if (request.Quantity <= 0 || request.Quantity > evt.Slot)
            {
                throw new InvalidQuantityException();
            }
            var user = await _accountService.GetCurrentUserAsync(userId)
                    ?? throw new ObjectNotFoundException($"{userId}");

            int totalCost = request.Quantity * evt.Price;
            if (user.wallet < totalCost)
                throw new Exception("Insufficient funds");
            user.wallet -= totalCost;
            await _accountService.ChangeUserMoney(user);
            for (int i = 0; i < request.Quantity; i++)
            {
                {
                    var ticket = new Booking
                    {
                        BookingId = await GenerateBookingId(),
                        EventId = request.EventID,
                        AccountId = user.Id,
                        Status = BookingStatus.Paid.ToString(),
                        CreatedDate = DateTime.UtcNow,
                    };
                    await _bookingRepository.CreateAsync(ticket);
                    await _eventService.UpdateEventSlotAsync(request.EventID, 1); // Decrease slot by 1 for each ticket booked
                }
            }
            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task TransferTicketAsync(TransferBookingRequest transferBookingRequest)
    {
        string bookingId = transferBookingRequest.BookingId;
        string userId = transferBookingRequest.UserId;
        string targetUserId = transferBookingRequest.TransferAccountId;
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            throw new ObjectNotFoundException($"{bookingId}");
        }
        if (booking.AccountId != userId)
        {
            throw new NotValidUserException();
        }
        //Check user có không trước khi luuw
        var user = await _accountService.GetCurrentUserAsync(targetUserId);
        if (user == null)
        {
            throw new ObjectNotFoundException($"{targetUserId}");
        }
        booking.AccountId = targetUserId;
        await _bookingRepository.UpdateAsync(booking);


    }
}