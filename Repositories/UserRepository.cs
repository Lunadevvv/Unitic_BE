using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Models;
using Unitic_BE.Repositories.Interfaces;

namespace Unitic_BE.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UniticDbContext? _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UniticDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context!.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context!.Users.FindAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _logger.LogInformation($"UserId: {user.UserId}");
            _context!.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
