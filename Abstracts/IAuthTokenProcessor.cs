using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts;

public interface IAuthTokenProcessor
{
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user, IList<string> roles);

    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);

}