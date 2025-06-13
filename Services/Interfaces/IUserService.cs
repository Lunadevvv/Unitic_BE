using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Dtos;
using Unitic_BE.Models;

namespace Unitic_BE.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<User?> GetUserRealByIdAsync(int id);
        Task<UserResponseDto?> UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
}
