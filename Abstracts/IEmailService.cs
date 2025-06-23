using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unitic_BE.Contracts;

namespace Unitic_BE.Abstracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(SendEmailRequest sendEmailRequest);
    }
}