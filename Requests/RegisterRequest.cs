using Unitic_BE.Enums;

namespace Unitic_BE.Requests;

public record RegisterRequest
{
    public string Mssv { get; init; } 
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string PhoneNumber { get; init; }
    public required string UniversityName { get; init; } // University name, can be used for student verification or other purposes.
}