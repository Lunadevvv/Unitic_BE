using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;
using Unitic_BE.Enums;

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
        public async Task<List<Event>> GetAllEventsByStatus(EventStatus status)
        {
            return await _context.Events.Where(u => u.Status == status).ToListAsync();
        }

        public async Task AddEventAsync(Event myEvent)
        {
            _context.Events.Add(myEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<Event?> GetEventByIdAsync(string eventId)
        {
            return await _context.Events.FirstOrDefaultAsync(c => c.EventID == eventId);
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

        public async Task UpdateEventSlotAsync(string eventId, int amount)
        {
            var myEvent = await GetEventByIdAsync(eventId);
            if (myEvent == null)
            {
                throw new KeyNotFoundException($"Event with ID {eventId} not found.");
            }
            myEvent.Slot -= amount;
            if (myEvent.Slot <= 0)
            {
                myEvent.Status = EventStatus.SoldOut; // Update status if slot is 0 or less
            }
            _context.Events.Update(myEvent);
            await _context.SaveChangesAsync();
        }
    }
}
