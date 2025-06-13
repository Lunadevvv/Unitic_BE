using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE.Models;

namespace Unitic_BE.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUser(string username, string password);
        Task Register(User user);
    }
}
