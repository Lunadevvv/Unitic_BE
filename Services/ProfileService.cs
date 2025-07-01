using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;
using Unitic_BE.DTOs.Requests;

namespace Unitic_BE.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly CustomValidator _customValidator;

        public ProfileService(UserManager<User> userManager, CustomValidator customValidator)
        {
            _userManager = userManager;
            _customValidator = customValidator;
        }

        public async Task<User> GetCurrentUserInformation(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw new Exception("User not found!");

            return user;
        }

        public async Task UpdateUserInformation(string userId, UpdateUserInformation updateUserInformation)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw new Exception("User not found!");

            var (errorLists, isValid) = await _customValidator.ValidateUpdateProfileAsync(updateUserInformation);
            
            if(!isValid)
                throw new Exception("Update Profile failed!");

            user.FirstName = updateUserInformation.FirstName;
            user.LastName = updateUserInformation.LastName;
            user.Mssv = updateUserInformation.Mssv;
            user.UniversityId = updateUserInformation.UniversityId;

            await _userManager.UpdateAsync(user);
        }
    }
}