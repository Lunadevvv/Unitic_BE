using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Constants;
using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<List<Event>> GetAllInProgressEvents();
        Task<List<Event>> GetAllSoldOutEvents();
        Task<List<Event>> GetAllCompletedEvent();
        Task<List<Event>> GetAllPrivateEvent();
        Task<List<Event>> GetAllCancelledEvent();
        Task<List<Event>> GetAllPublishedEvent();

        Task AddEventAsync(Event myEvent);

        Task<Event?> GetEventByIdAsync(string id);

        Task UpdateEventAsync(Event myEvent);


        Task<string> GetLastId();

        Task<Event> GetEventByNameAsync(string name);


    }
}
