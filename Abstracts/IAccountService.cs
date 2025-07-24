using Unitic_BE.Entities;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Enums;
using Unitic_BE.DTOs.Responses;

namespace Unitic_BE.Abstracts;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest registerRequest);
    Task<string> LoginAsync(LoginRequest loginRequest);
    Task<string> ForgotPassword(string email);
    Task ResetPassword(string? userId, string newPassword);
    Task ChangePassword(string? userId, ChangePasswordRequest changePasswordRequest);
    Task<User> GetCurrentUserAsync(string userId);
    Task<bool> CheckMoneySufficent(int money, int userMoney);
    Task<bool> ChangeUserMoney(User user);
    Task<bool> UpdateUserWallet(User user, int money);
    Task<List<AccountResponse>> GetAllUsers();
    Task<AccountResponse> GetUserById(string accountId);
    Task UpdateAccountRoleAsync(string accountId, Role role);
}