using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.Repositories;

namespace Unitic_BE.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IAuthTokenProcessor _authTokenProcessor;

        public GoogleService(IConfiguration configuration, UserManager<User> userManager, IUserRepository userRepository, IAuthTokenProcessor authTokenProcessor)
        {
            _configuration = configuration;
            _userManager = userManager;
            _userRepository = userRepository;
            _authTokenProcessor = authTokenProcessor;
        }
        public void ConfigureGoogleService(IServiceCollection services)
        {
            services.AddAuthentication()
                .AddGoogleOpenIdConnect("GoogleConnect", options =>
            {
                options.ClientId = _configuration["Google:ClientId"];
                options.ClientSecret = _configuration["Google:ClientSecret"];
                options.Scope.Add("email");
                options.Scope.Add("profile");
            });
        }

        public async Task<string> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                throw new ArgumentNullException(nameof(claimsPrincipal), "Claims principal is null");
            }

            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                throw new InvalidOperationException("Email is null.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // If user does not exist, create a new user
                user = new User
                {
                    Id = await GenerateUserId(), // Assuming you have a method to generate a unique user ID
                    Email = email,
                    UserName = email, // or any other unique identifier
                    FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                    LastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                    UniversityId = "Uni0001",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user from Google login.");
                }
                var addRoleResult = await _userManager.AddToRoleAsync(user, GetStringIdentityRoleName(Role.User));

            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            var (jwtToken, expirationDateInUtc) = _authTokenProcessor.GenerateJwtToken(user, roles);

            _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);

            return jwtToken;
        }

        private string GetStringIdentityRoleName(Role role)
        {
            return role switch
            {
                Role.Moderator => IdentityRoleConstants.Moderator,
                Role.Staff => IdentityRoleConstants.Staff,
                Role.Organizer => IdentityRoleConstants.Organizer,
                Role.Admin => IdentityRoleConstants.Admin,
                Role.User => IdentityRoleConstants.User,
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Provided role is not supported.")
            };
        }
        
        private async Task<string> GenerateUserId()
        {
            string lastId = await _userRepository.GetLastId();
            if (lastId == null) return "User0001";
            int id = int.Parse(lastId.Substring(4)) + 1; // lấy id cuối cùng và cộng thêm 1
            string generatedId = "User" + id.ToString("D4");
            return generatedId;
        }
    }
}