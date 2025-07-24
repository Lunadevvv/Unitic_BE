using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;

namespace Unitic_BE.Repositories
{
    public class OrganizerRepository : IOrganizerRepository
    {
        // Assuming you have a DbContext injected here
        private readonly ApplicationDbContext _context;

        public OrganizerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignOrganizerToEvent(string userId, string eventId)
        {
            //Create a new organizer with the provided userId and eventId
            var organizer = new Organizer
            {
                UserId = userId,
                EventID = eventId,
                OrganizerId = await GenerateOrganizerId()
            };
            _context.Organizers.Add(organizer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveOrganizerFromEvent(string organizerId)
        {
            var organizer = await _context.Organizers.FindAsync(organizerId);
            if (organizer == null) return false;

            //remove organizer row
            _context.Organizers.Remove(organizer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Event>> GetEventsByOrganizer(string userId)
        {
            return await _context.Organizers
                .Where(o => o.UserId == userId)
                .Select(o => o.Event)
                .ToListAsync();
        }

        public async Task<string> GetLastOrganizerIdAsync()
        {
            return await _context.Organizers
                .OrderByDescending(o => o.OrganizerId)
                .Select(o => o.OrganizerId)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GenerateOrganizerId()
        {
            string lastId = await GetLastOrganizerIdAsync();
            if (lastId == null) return "Orga0001";
            int id = int.Parse(lastId.Substring(4)) + 1; // lấy id cuối cùng và cộng thêm 1
            string generatedId = "Orga" + id.ToString("D4");
            return generatedId;
        }
    }
}