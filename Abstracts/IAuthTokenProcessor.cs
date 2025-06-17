using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts;

public interface IAuthTokenProcessor
{
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user, IList<string> roles);
    (string jwtToken, DateTime expiresAtUtc) GenerateExpiredJwtToken(User user, IList<string> roles);
    //string GenerateRefreshToken();
    //void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
    void WriteAuthTokenAsHeader(string token, DateTime expiration);
    void WriteAuthTokenAsLogOutToken(string token,
    DateTime expiration);
}