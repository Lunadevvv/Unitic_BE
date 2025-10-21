using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts;

public interface IUserRepository
{
    Task<string> GetUniversityIdByNameAsync(string universityName);
    Task<User> GetUserById(string userId);
    Task<List<User>> GetAllUsers();
    Task<string> GetLastId();
}