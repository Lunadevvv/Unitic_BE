using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Entities;
using Unitic_BE.Requests;

namespace Unitic_BE.Abstracts
{
    public interface IEventService
    {
        
        Task SoldOutEventAsync(string id);
        Task<List<Event>> GetAllEvents();
        Task<List<Event>> GetAllInProgressEvents();
        Task<List<Event>> GetAllSoldOutEvents();
        Task<List<Event>> GetAllCompletedEvents();
        Task<List<Event>> GetAllCancelledEvents();
        Task<List<Event>> GetAllPublishedEvents();
        Task<List<Event>> GetAllPrivateEvents();
        Task AddEventAsync(EventRequest myEventRequest);
        Task<Event?> GetEventByIdAsync(string id);
        Task UpdateEventAsync(string id, EventRequest myEventRequest);
        Task CancelledEventAsync(string id);
        Task PrivateOrPublishedEventAsync(string id);
        Task InProgressEventAsync(string id);
        Task CompletedEventAsync(string id);
    }
}
