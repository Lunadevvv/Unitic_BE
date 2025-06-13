using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Models;
using Unitic_BE.Repositories.Interfaces;

namespace Unitic_BE.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UniticDbContext _dbContext;
        public AuthRepository(UniticDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetUser(string username, string password)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
            return user;
        }

        public async Task Register(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
