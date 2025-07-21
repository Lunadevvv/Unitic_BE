using Microsoft.EntityFrameworkCore;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;

namespace Unitic_BE.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ApplicationDbContext _context;

    public FeedbackRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Feedback>> GetAllAsync()
    {
        return await _context.Feedbacks.ToListAsync();
    }

    public async Task<Feedback> GetByIdAsync(string id)
    {
        return await _context.Feedbacks.FindAsync(id);
    }

    public async Task<Feedback> CreateAsync(Feedback feedback)
    {
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();
        return feedback;
    }

    public async Task UpdateAsync(Feedback feedback)
    {
        _context.Feedbacks.Update(feedback);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var feedback = await _context.Feedbacks.FindAsync(id);
        if (feedback != null)
        {
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
        }

    }
    
    public async Task<Feedback?> GetLastFeedback()
    {
        return await _context.Feedbacks
            .OrderByDescending(b => b.FeedbackId)
            .FirstOrDefaultAsync();
    }
}