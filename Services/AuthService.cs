
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Unitic_BE.Dtos;
using Unitic_BE.Models;
using Unitic_BE.Repositories;
using Unitic_BE.Repositories.Interfaces;
using Unitic_BE.Services.Interfaces;



namespace Unitic_BE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        public AuthService(IAuthRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<User?> GetUser(LoginDto loginUser)
        {
            var user = await _repo.GetUser(loginUser.UserName, loginUser.Password);
            return user;
        }

        public async Task Register(RegisterDto register)
        {

            var user = _mapper.Map<User>(register);

            await _repo.Register(user);
        }
    }
}
