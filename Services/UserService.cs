using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Dtos;
using Unitic_BE.Models;
using Unitic_BE.Repositories;
using Unitic_BE.Repositories.Interfaces;
using Unitic_BE.Services.Interfaces;

namespace Unitic_BE.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        public UserService(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task DeleteUserAsync(int id)
        {
            await _repo.DeleteUserAsync(id);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            return _mapper.Map<UserResponseDto?>(user);
        }

        public async Task<User?> GetUserRealByIdAsync(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            return user;
        }

        public async Task<UserResponseDto?> UpdateUserAsync(User user)
        {
            await _repo.UpdateUserAsync(user);
            var updatedUser = await _repo.GetUserByIdAsync(user.UserId);
            return _mapper.Map<UserResponseDto?>(updatedUser);
        }
    }
}
