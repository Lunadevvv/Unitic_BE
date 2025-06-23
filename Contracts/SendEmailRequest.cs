namespace Unitic_BE.Contracts
{
    public record SendEmailRequest(string Recipient, string Subject, string Body);
}