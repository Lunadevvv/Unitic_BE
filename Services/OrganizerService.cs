using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;
using Unitic_BE.Enums;

namespace Unitic_BE.Services
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly UserManager<User> _userManager;
        public OrganizerService(IOrganizerRepository organizerRepository, UserManager<User> userManager)
        {
            _userManager = userManager;
            _organizerRepository = organizerRepository;
        }
        public async Task<bool> AssignOrganizerToEvent(string userId, string eventId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            if (user.Role != Role.Organizer)
            {
                throw new Exception("User is not an organizer.");
            }
            //Create a new organizer with the provided userId and eventId
            return await _organizerRepository.AssignOrganizerToEvent(userId, eventId);
        }

        public async Task<List<Event>> GetEventsByOrganizer(string userId)
        {
            return await _organizerRepository.GetEventsByOrganizer(userId);
        }

        public Task<List<User>> GetOrganizersByEvent(string eventId)
        {
            return _organizerRepository.GetOrganizersByEvent(eventId);
        }

        public async Task<bool> RemoveOrganizerFromEvent(string organizerId)
        {
            //Remove the organizer from the event
            return await _organizerRepository.RemoveOrganizerFromEvent(organizerId);
        }
    }
}