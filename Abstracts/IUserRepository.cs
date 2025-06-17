using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts;

public interface IUserRepository
{
    Task<User?> GetUserByTokenAsync(string refreshToken);
    Task<string> GetUniversityIdByNameAsync(string universityName);
    Task<User> GetUserById(string userId);
    Task<string> GetLastId();
}