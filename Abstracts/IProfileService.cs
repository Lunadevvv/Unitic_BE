using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal.Mappers;
using Unitic_BE.Entities;
using Unitic_BE.DTOs.Requests;

namespace Unitic_BE.Abstracts
{
    public interface IProfileService
    {
        Task<User> GetCurrentUserInformation(string userId);
        Task UpdateUserInformation(string userId, UpdateUserInformation updateUserInformation);
    }
}