using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.Exceptions;
using Unitic_BE.Requests;

namespace Unitic_BE.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repo;
        private readonly CustomValidator _validator;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IEventJobScheduler _scheduler;
        public EventService(IEventRepository repo, CustomValidator validator, ICategoryRepository categoryRepository, IEventJobScheduler scheduler)
        {
            _validator = validator;
            _repo = repo;
            _categoryRepo = categoryRepository;
            _scheduler = scheduler;
        }
        public Task<List<Event>> GetAllEvents()
        {
            return _repo.GetAllEventsAsync();
        }
        public Task<List<Event>> GetAllEventsByStatus(EventStatus status)
        {
            return _repo.GetAllEventsByStatus(status);
        }
        public async Task AddEventAsync(EventRequest myEventRequest)
        {
            // Trim all string properties in the request
            StringTrimmerExtension.TrimAllString(myEventRequest);
            //Check xem myEventName tồn tại chưa
            var myEventExists = await _repo.GetEventByNameAsync(myEventRequest.Name);
            if (myEventExists != null)
            {
                throw new ObjPropertyAlreadyExists(myEventRequest.Name);
            }
            //validate the request
            var categoryList = await _categoryRepo.GetAllCategoriesAsync();
            var (errors, isValid) = _validator.ValidateEventRequest(myEventRequest, categoryList.Select(u => u.Name).ToList());
            if (!isValid)
            {
                throw new UpdateAddFailedException(errors);
            }
            var category = await _categoryRepo.GetCategoryByNameAsync(myEventRequest.CategoryName);
            // Create a new Event entity from the request
            Event myEvent = new Event
            {
                EventID = await GenerateEventId(),
                Name = myEventRequest.Name,
                Status = EventStatus.Private,
                Description = myEventRequest.Description,
                Date_Start = DateTime.Parse(myEventRequest.Date_Start),
                Date_End = DateTime.Parse(myEventRequest.Date_End),
                Price = myEventRequest.Price,
                CateID = category.CateID,
                Slot = myEventRequest.Slot 
            };
            // Add the myEvent to the repository
            await _repo.AddEventAsync(myEvent);

        }
        public async Task<Event?> GetEventByIdAsync(string id)
        {
            // Retrieve the myEvent by ID
            var myEvent = await _repo.GetEventByIdAsync(id);
            if (myEvent == null)
            {
                throw new ObjectNotFoundException($"Event with id {id} is");
            }
            return myEvent;
        }
        public async Task UpdateEventAsync(string id, EventRequest myEventRequest)
        {
            // Trim all string properties in the request
            StringTrimmerExtension.TrimAllString(myEventRequest);
            // Check xem myEvent đã tồn tại chưa
            var myEvent = await _repo.GetEventByIdAsync(id);
            if (myEvent == null)
            {
                throw new ObjectNotFoundException($"Event with id {id} not found.");
            }
            //check myEventName tồn tại chưa
            var myEventExists = await _repo.GetEventByNameAsync(myEventRequest.Name);
            if (myEventExists != null && myEventExists.EventID != myEvent.EventID)
            {
                throw new ObjPropertyAlreadyExists(myEventRequest.Name);
            }
            //validate
        
            var categoryList = await _categoryRepo.GetAllCategoriesAsync();
            var (errors, isValid) = _validator.ValidateEventRequest(myEventRequest, categoryList.Select(u => u.Name).ToList());
            if (!isValid)
            {
                throw new UpdateAddFailedException(errors);
            }
            // Update the myEvent properties
            var category = await _categoryRepo.GetCategoryByNameAsync(myEventRequest.CategoryName);
            myEvent.Name = myEventRequest.Name;
            myEvent.Description = myEventRequest.Description;
            myEvent.Date_Start = DateTime.Parse(myEventRequest.Date_Start);
            myEvent.Date_End = DateTime.Parse(myEventRequest.Date_End);
            myEvent.Price = myEventRequest.Price;
            myEvent.CateID = category.CateID;
            myEvent.Slot = myEventRequest.Slot;
            // Update the myEvent in the repository
            await _repo.UpdateEventAsync(myEvent);

            // if date start is changed, delete the job if it exists and create new job
            if (myEvent.Status == EventStatus.Published)
            {
                await _scheduler.DeleteStatusJobAsync(id);
                await _scheduler.ScheduleUpdateStatusJobAsync(id, myEvent.Date_Start);
            }
        }
        
        private async Task<string> GenerateEventId()
        {
            string lastId = await _repo.GetLastId();
            if (lastId == null) return "Event0001";
            int id = int.Parse(lastId.Substring(5)) + 1; // lấy id cuối cùng và cộng thêm 1
            string generatedId = "Event" + id.ToString("D4");
            return generatedId;
        }

        public async Task UpdateEventStatusAsync(string id, EventStatus status)
        {
            var myEvent = await _repo.GetEventByIdAsync(id);
            if (myEvent == null)
            {
                throw new ObjectNotFoundException($"Event with id {id}");
            }

            myEvent.Status = status;
            await _repo.UpdateEventAsync(myEvent);

            //gọi scheduler tạo job nếu update lên published
            if (status == EventStatus.Published)
                await _scheduler.ScheduleUpdateStatusJobAsync(id, myEvent.Date_Start);
            //ko phải published thì xóa job
            else
                await _scheduler.DeleteStatusJobAsync(id);

        }

    }
}

