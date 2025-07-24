using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts
{
    public interface IOrganizerService
    {
        Task<bool> AssignOrganizerToEvent(string userId, string eventId);
        Task<bool> RemoveOrganizerFromEvent(string organizerId);
        Task<List<User>> GetOrganizersByEvent(string eventId);
        Task<List<Event>> GetEventsByOrganizer(string userId);
    }
}