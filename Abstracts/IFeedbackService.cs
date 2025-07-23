using System.Collections.Generic;
using System.Threading.Tasks;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;

public interface IFeedbackService
{
    Task<IEnumerable<Feedback>> GetAllAsync();
    Task<Feedback> GetByIdAsync(string feedbackId);
    Task<List<Feedback>> GetByEventIdAsync(string eventId);
    Task CreateAsync(FeedbackRequest request);
    Task UpdateAsync(Feedback feedback);
    // Task DeleteAsync(string id);
}