using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts;

public interface IAuthTokenProcessor
{
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user, IList<string> roles);
    (string jwtToken, DateTime expiresAtUtc) GenerateResetJwtToken(User user);

    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
    
}