using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Unitic_BE.Abstracts;
using Unitic_BE.Contracts;
using Unitic_BE.Options;

namespace Unitic_BE.Services
{
    public class EmailService : IEmailService
    {
        private readonly GmailOptions _gmailOptions;

        public EmailService(IOptions<GmailOptions> gmailOptions)
        {
            _gmailOptions = gmailOptions.Value;
        }

        public async Task SendEmailAsync(SendEmailRequest sendEmailRequest)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_gmailOptions.Email),
                Subject = sendEmailRequest.Subject,
                Body = sendEmailRequest.Body
            };

            mailMessage.To.Add(sendEmailRequest.Recipient);

            using var smtpClient = new SmtpClient();
            smtpClient.Host = _gmailOptions.Host;
            smtpClient.Port = _gmailOptions.Port;
            smtpClient.Credentials = new NetworkCredential(
                _gmailOptions.Email, _gmailOptions.Password
            );
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}