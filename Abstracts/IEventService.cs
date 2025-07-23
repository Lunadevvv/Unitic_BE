using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.DTOs.Requests;

namespace Unitic_BE.Abstracts
{
    public interface IEventService
    {
        Task UpdateEventStatusAsync(string eventId, EventStatus status);
        Task<List<Event>> GetAllEvents();
        Task<List<Event>> GetAllEventsByStatus(EventStatus status);
        Task AddEventAsync(EventRequest myEventRequest);
        Task<Event?> GetEventByIdAsync(string id);
        Task UpdateEventAsync(string id, EventRequest myEventRequest);
        Task<Event> CheckEventStatusAsync(string id);
        Task UpdateEventSlotAsync(string eventId, int amount);
    }
}
