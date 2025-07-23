using System.Collections.Generic;
using System.Threading.Tasks;
using Unitic_BE.Entities;
namespace Unitic_BE.Abstracts;
public interface IFeedbackRepository
{
    Task<IEnumerable<Feedback>> GetAllAsync();
    Task<List<Feedback>> GetByEventIdAsync(string eventId);
    Task<Feedback> GetByIdAsync(string feedbackId);
    Task<Feedback> CreateAsync(Feedback feedback);
    Task UpdateAsync(Feedback feedback);
    // Task DeleteAsync(string id);

    Task<Feedback?> GetLastFeedback();
}