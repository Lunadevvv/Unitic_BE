
using Microsoft.AspNetCore.Identity;
using System.Net;
using System;
using System.Text.RegularExpressions;
using Unitic_BE.Entities;
using Unitic_BE.Exceptions;
using System.ComponentModel;
using Unitic_BE.Abstracts;
using Unitic_BE.Requests;
using Unitic_BE.Requests;

public class CustomValidator
{

    public (List<string>, bool) ValidateUser(RegisterRequest register, List<string> universityNames)
    {
        //email, password ko cần check null
        var errors = new List<ErrorResponse>();

        //email
        if (!Regex.IsMatch(register.Email, @"^(?!.*\s).+@(gmail\.com|fpt\.edu\.vn)$"))
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nmail must be @fpt.edu.vn or @gmail.vn"
            });
        }


        //password
        {
            if (register.Password.Length < 8)
            {
                errors.Add(new ErrorResponse
                {

                    Description = "\nPassword must be at minimum 8 characters long"
                });
            }
            if (register.Password.Length > 16)
            {
                errors.Add(new ErrorResponse
                {

                    Description = "\nPassword must be at maximum 16 characters long"
                });
            }
        }
        //FirstName
        if (string.IsNullOrWhiteSpace(register.FirstName))
        {
            errors.Add(new ErrorResponse
            {

                Description = "\nFirst name can't be null"
            });
        }
        //chỉ chứa chữ cái
        // \p{L} là ký tự chữ cái Unicode 
        if (!Regex.IsMatch(register.FirstName, @"^\p{L}+$"))
            errors.Add(new ErrorResponse
            {

                Description = "\nFirst name contains only characters"
            });

        //LastName
        if (string.IsNullOrWhiteSpace(register.LastName))
        {
            errors.Add(new ErrorResponse
            {

                Description = "\nLast name can't be null"
            });
        }
        if (!Regex.IsMatch(register.LastName, @"^\p{L}+$"))
            errors.Add(new ErrorResponse
            {

                Description = "\nLast name contains only characters"
            });


        //UniversityName
        if (string.IsNullOrWhiteSpace(register.UniversityName))
        {
            errors.Add(new ErrorResponse
            {

                Description = "\nUniversity Name can't be null"
            });
        }
        bool a = false;
        foreach (var university in universityNames)
        {
            if (university.Equals(register.UniversityName))
            {
                a = true;
                break;
            }
        }
        if (!a)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nUniversity available:\n" + string.Join("\n", universityNames.Select((name, index) => $"{index + 1}. {name}"))
            });
        }
        return errors.Any() ? (errors.Select(e => e.Description).ToList(), false) : (new List<string>(), true);
    }
    public (List<string>, bool) ValidateUniversity(UniversityRequest request)
    {
        var errors = new List<ErrorResponse>();
        if (!Regex.IsMatch(request.Name, @"^Đại học( [A-ZÀ-Ỵ][a-zà-ỹ]*)+$"))
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nUniversity name must start with 'Đại học' each character at the start of university name must be uppercase"
            });
        }
        return errors.Any() ? (errors.Select(e => e.Description).ToList(), false) : (new List<string>(), true);
    }

    public (List<string>, bool) ValidatePassword(string password)
    {
        var errors = new List<ErrorResponse>();
        //password

        if (password.Length < 8)
        {
            errors.Add(new ErrorResponse
            {

                Description = "\nPassword must be at minimum 8 characters long"
            });
        }
        if (password.Length > 16)
        {
            errors.Add(new ErrorResponse
            {

                Description = "\nPassword must be at maximum 16 characters long"
            });
        }
        return errors.Any() ? (errors.Select(e => e.Description).ToList(), false) : (new List<string>(), true);
    }
    public (List<string>, bool) ValidateEventRequest(EventRequest eventRequest, List<string> categoryNames)
    {
        var errors = new List<ErrorResponse>();

        //description
        if (string.IsNullOrWhiteSpace(eventRequest.Description))
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nDescription can't be null or empty"
            });
        }
        //eventDate
        if (eventRequest.Date_Start == null)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nStart date can't be null"
            });
        }
        if (eventRequest.Date_End == null)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nEnd date can't be null"
            });
        }

        if (!string.IsNullOrWhiteSpace(eventRequest.Date_Start) &&
            !Regex.IsMatch(eventRequest.Date_Start, @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$"))
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nStart date must be in the format yyyy-MM-ddTHH:mm:ss"
            });
        }

        if (!string.IsNullOrWhiteSpace(eventRequest.Date_End) &&
            !Regex.IsMatch(eventRequest.Date_End, @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$"))
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nEnd date must be in the format yyyy-MM-ddTHH:mm:ss"
            });
        }
        bool dateStart = false, dateEnd = false;
        DateTime Date_Start = DateTime.Now, Date_End = DateTime.Now;
        if (!string.IsNullOrWhiteSpace(eventRequest.Date_Start))
        {
            dateStart = DateTime.TryParse(eventRequest.Date_Start, out Date_Start);
        }
        if (!string.IsNullOrWhiteSpace(eventRequest.Date_End))
        {
            dateEnd = DateTime.TryParse(eventRequest.Date_End, out Date_End);
        }

        if (dateStart && Date_Start < DateTime.Now)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nStart date must be greater than current date"
            });
        }
        if (dateEnd && Date_End < Date_Start)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nEnd date must be greater than start date"
            });
        }
        if (dateStart && dateEnd && Date_Start == Date_End)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nStart date must be different from end date"
            });
        }

        //event price
        if (eventRequest.Price == null)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nPrice can't be null"
            });
        }
        if (eventRequest.Price < 0)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nPrice must be greater than or equal to 0"
            });
        }

        //eventName
        if (string.IsNullOrWhiteSpace(eventRequest.Name))
        {
            errors.Add(new ErrorResponse
            {

                Description = "\nEvent name can't be null or empty"
            });
        }
        bool a = false;
        foreach (var category in categoryNames)
        {
            if (category.Equals(eventRequest.CategoryName))
            {
                a = true;
                break;
            }
        }
        if (!a)
        {
            errors.Add(new ErrorResponse
            {
                Description = "\nCategory available:\n" + string.Join("\n", categoryNames.Select((name, index) => $"{index + 1}. {name}"))
            });
        }
        return errors.Any() ? (errors.Select(e => e.Description).ToList(), false) : (new List<string>(), true);
    }

}
