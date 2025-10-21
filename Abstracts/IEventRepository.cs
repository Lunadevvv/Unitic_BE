using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Constants;
using Unitic_BE.Entities;
using Unitic_BE.Enums;

namespace Unitic_BE.Abstracts
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<List<Event>> GetAllEventsByStatus(EventStatus status);
        Task AddEventAsync(Event myEvent);
        Task<Event?> GetEventByIdAsync(string id);
        Task UpdateEventAsync(Event myEvent);
        Task UpdateEventSlotAsync(string eventId, int amount);
        Task<string> GetLastId();
        Task<Event> GetEventByNameAsync(string name);

    }
}
