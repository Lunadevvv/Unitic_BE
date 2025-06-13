using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Tree;
using Unitic_BE.Dtos;
using Unitic_BE.Models;

namespace Unitic_BE.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> GetUser(LoginDto user);
        Task Register(RegisterDto register);
    }
}
