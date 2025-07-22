using System.Collections.Generic;
using System.Threading.Tasks;
using Unitic_BE.Entities;

public interface IFeedbackService
{
    Task<IEnumerable<Feedback>> GetAllAsync();
    Task<Feedback> GetByIdAsync(string id);
    Task CreateAsync(Feedback feedback, string eventId);
    Task UpdateAsync(Feedback feedback);
    Task DeleteAsync(string id);
}