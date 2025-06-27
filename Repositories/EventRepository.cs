using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Entities;

namespace Unitic_BE.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;
        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }
        public async Task<List<Event>> GetAllCompletedEvent()
        {
            return await _context.Events.Where(u => u.Status == EventStatusConstant.Completed).ToListAsync();
        }
        public async Task<List<Event>> GetAllCancelledEvent()
        {
            return await _context.Events.Where(u => u.Status == EventStatusConstant.Cancelled).ToListAsync();
        }
        public async Task<List<Event>> GetAllPublishedEvent()
        {
            return await _context.Events.Where(u => u.Status == EventStatusConstant.Published).ToListAsync();
        }
        public async Task<List<Event>> GetAllPrivateEvent()
        {
            return await _context.Events.Where(u => u.Status == EventStatusConstant.Private).ToListAsync();
        }

        public async Task AddEventAsync(Event myEvent)
        {
            _context.Events.Add(myEvent);
            await _context.SaveChangesAsync();

        }
        public async Task<Event?> GetEventByIdAsync(string id)
        {
            return await _context.Events.FirstOrDefaultAsync(c => c.EventID == id);
        }
        public async Task UpdateEventAsync(Event myEvent)
        {
            _context.Events.Update(myEvent);
            await _context.SaveChangesAsync();
        }

        
        public async Task<string> GetLastId()
        {
            string id = await _context.Events
                .OrderByDescending(c => c.EventID)
                .Select(c => c.EventID)
                .FirstOrDefaultAsync();
            return id;
        }
        public async Task<Event> GetEventByNameAsync(string name)
        {
            return await _context.Events
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<List<Event>> GetAllSoldOutEvents()
        {
            return await _context.Events.Where(u => u.Status == EventStatusConstant.SoldOut).ToListAsync();
        }
        public async Task<List<Event>> GetAllInProgressEvents()
        {
            return await _context.Events.Where(u => u.Status == EventStatusConstant.InProgress).ToListAsync();
        }
    }
}
