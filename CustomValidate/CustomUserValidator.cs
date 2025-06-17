using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Unitic_BE.Entities;

public class CustomUserValidator : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        //email, password ko cần check null
        var errors = new List<IdentityError>();
        //email
        if (!Regex.IsMatch(user.Email, @"^(?!.*\s).+@(gmail\.com|fpt\.edu\.vn)$"))
        {
            errors.Add(new IdentityError
            {
                Code = "WrongEmailFormat",
                Description = "mail must be @fpt.edu.vn or @gmail.vn"
            });
        }

        //password
        if (user.Password.Length < 8)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Password must be at minimum 8 characters long"
            });
        }
        if (user.Password.Length >16)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordTooLong",
                Description = "Password must be at maximum 16 characters long"
            });
        }

        //FirstName
        if (string.IsNullOrWhiteSpace(user.FirstName))
        {
            errors.Add(new IdentityError
            {
                Code = "FirstNameNull",
                Description = "First name can't be null"
            });
        }
        //chỉ chứa chữ cái
        // \p{L} là ký tự chữ cái Unicode 
        if (!Regex.IsMatch(user.FirstName, @"^\p{L}+$"))
            errors.Add(new IdentityError
            {
                Code = "InvalidFirstName",
                Description = "First name contains only characters"
            });

        //LastName
        if (string.IsNullOrWhiteSpace(user.LastName))
        {
            errors.Add(new IdentityError
            {
                Code = "LastNameNull",
                Description = "Last name can't be null"
            });
        }
        if (!Regex.IsMatch(user.LastName, @"^\p{L}+$"))
            errors.Add(new IdentityError
            {
                Code = "InvalidLastName",
                Description = "Last name contains only characters"
            });

        //PhoneNumber
        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            errors.Add(new IdentityError
            {
                Code = "PhoneNumberNull",
                Description = "Phone number can't be null"
            });
        }

        //phone number length
        if (user.PhoneNumber.Length < 10 || user.PhoneNumber.Length > 10)
        {
            errors.Add(new IdentityError
            {
                Code = "PhoneNumberTooShort",
                Description = "Phone number must be at 10 digits"
            });
        }

        //phone number format
        if (!Regex.IsMatch(user.PhoneNumber, @"^(0|\+84)(3|5|7|8|9)"))
        {
            errors.Add(new IdentityError
            {
                Code = "PhoneNumberWrongFormat",
                Description = "Invalid phone number format"
            });
        }

        //UniversityId
        if (string.IsNullOrWhiteSpace(user.UniversityId))
        {
            errors.Add(new IdentityError
            {
                Code = "UniversityIdNullWrong",
                Description = "University ID can't be null or wrong"
            });
        }
        //int c = 0;
        //c++;
        //if(c==2)
        //user.Password = string.Empty; // Clear password to avoid storing it in memory after validation
        return Task.FromResult(errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success);
    }
}
