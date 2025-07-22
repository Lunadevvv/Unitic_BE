using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Unitic_BE.Enums;
using Unitic_BE.Exceptions;

namespace Unitic_BE.Entities;

public class User : IdentityUser<string>
{
    // public string Password { get; set; } = string.Empty; // Password is not stored in the database, only the hash is stored.
    public int wallet { get; set; } = 0; // Wallet is initialized to 0, can be used for storing user balance or credits.
    public string Mssv { get; set; } = string.Empty;
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Role Role { get; set; } = Role.User; // Default role is User, can be changed to Admin or other roles as needed.
    public string? UniversityId { get; set; } // FK từ bảng university

    [ForeignKey("UniversityId")]
    public University? University { get; set; } // Navigation property
    //factory method to create a new user instance
    public static User Create(string id, string mssv,string email, string firstName, string lastName, string universityId)
    {

        //chuyển university name thành university id
        return new User
        {
            Id = id, // Id is the primary key, can be Guid or string
            Mssv = mssv,
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            UniversityId = universityId, // Assuming universityName is the ID of the university
        };
    }
    
    public override string ToString()
    {
        return FirstName + " " + LastName;
    }
}