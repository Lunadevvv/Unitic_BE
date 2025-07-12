using Unitic_BE.Entities;
using Unitic_BE.DTOs.Requests;

namespace Unitic_BE.Abstracts;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest registerRequest);
    Task<string> LoginAsync(LoginRequest loginRequest);
    Task<string> ForgotPassword(string email);
    Task ResetPassword(string? userId, string newPassword);
    Task ChangePassword(string? userId, ChangePasswordRequest changePasswordRequest);
    Task<User> GetCurrentUserAsync(string userId);
    // Task RegisterRoleAsync(string role, RegisterRequest registerRequest);
    Task<bool> CheckMoneySufficent(int money, int userMoney);
    Task<bool> ChangeUserMoney(User user);

}