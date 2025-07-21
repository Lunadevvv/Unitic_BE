using System.Collections.Generic;
using System.Threading.Tasks;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _repository;
    private readonly IEventRepository _eventRepository;

    public FeedbackService(IFeedbackRepository repository, IEventRepository eventRepository)
    {
        _repository = repository;
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<Feedback>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Feedback> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> CreateAsync(Feedback feedback, string eventId)
    {
        var feedbackEvent = await _eventRepository.GetEventByIdAsync(eventId);
        bool result = false;
        if (feedbackEvent != null)
        {
            var currentDate = DateTime.Now;
            if (feedbackEvent.Date_End <= currentDate)
            {
                feedback.FeedbackId = await GenerateFeedbackId();
                feedback.CreatedDate = currentDate;
                result = (await _repository.CreateAsync(feedback)) != null;
            }
        }
        return result;
    }

    public async Task UpdateAsync(Feedback feedback)
    {
        await _repository.UpdateAsync(feedback);
    }

    public async Task DeleteAsync(string id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<string> GenerateFeedbackId()
    {
        var lastFeedback = await _repository.GetLastFeedback();
        if (lastFeedback == null) return "Feedback0001";
        int id = int.Parse(lastFeedback.FeedbackId.Substring(lastFeedback.FeedbackId.Length - 4)) + 1; // lấy id cuối cùng và cộng thêm 1
        string generatedId = "Feedback" + id.ToString("D4");
        return generatedId;
    }
}