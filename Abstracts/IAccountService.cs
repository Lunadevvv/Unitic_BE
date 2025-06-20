using Unitic_BE.Entities;
using Unitic_BE.Requests;

namespace Unitic_BE.Abstracts;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest registerRequest);
    Task<string> LoginAsync(LoginRequest loginRequest);
    Task<string> ForgotPassword(string email);
    Task ResetPassword(string? userId, string newPassword);
    Task ChangePassword(string? userId, ChangePasswordRequest changePasswordRequest);
}