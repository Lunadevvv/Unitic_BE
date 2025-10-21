using System.Collections.Generic;
using System.Threading.Tasks;
using Unitic_BE.Abstracts;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;
using Unitic_BE.Enums;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IBookingRepository _bookingRepository;

    public FeedbackService(IFeedbackRepository repository, IEventRepository eventRepository, IBookingRepository bookingRepository)
    {
        _repository = repository;
        _eventRepository = eventRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<Feedback>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Feedback> GetByIdAsync(string eventId)
    {
        return await _repository.GetByIdAsync(eventId);
    }

    public async Task CreateAsync(FeedbackRequest request)
    {
        try
        {
            var evt = await _eventRepository.GetEventByIdAsync(request.EventId);
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
            var currentDate = DateTime.Now;
            if (evt == null)
                throw new Exception("Can't get event");
            if (booking == null)
                throw new Exception("Can't get event");
            if (evt.EventID != request.EventId)
                throw new Exception("This is not user event");
            if (evt.Status != EventStatus.Completed)
                throw new Exception("Event haven't finish");
            if (!string.IsNullOrEmpty(booking.FeedbackId))
            {
                throw new Exception("Already feedback");
            }
            Feedback feedback = new Feedback
            {
                FeedbackId = await GenerateFeedbackId(),
                BookingId = request.BookingId,
                Content = request.Content,
                EventID = request.EventId,
                CreatedDate = currentDate
            };
            await _repository.CreateAsync(feedback);
            booking.FeedbackId = feedback.FeedbackId;
            await _bookingRepository.UpdateAsync(booking);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
    }

    public async Task UpdateAsync(Feedback feedback)
    {
        await _repository.UpdateAsync(feedback);
    }

    // public async Task DeleteAsync(string id)
    // {
    //     await _repository.DeleteAsync(id);
    // }

    public async Task<string> GenerateFeedbackId()
    {
        var lastFeedback = await _repository.GetLastFeedback();
        if (lastFeedback == null) return "Feedback0001";
        int id = int.Parse(lastFeedback.FeedbackId.Substring(lastFeedback.FeedbackId.Length - 4)) + 1; // lấy id cuối cùng và cộng thêm 1
        string generatedId = "Feedback" + id.ToString("D4");
        return generatedId;
    }

    public Task<List<Feedback>> GetByEventIdAsync(string eventId)
    {
        return _repository.GetByEventIdAsync(eventId);
    }
}