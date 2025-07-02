using Unitic_BE.DTOs.Requests;
using Unitic_BE.DTOs.Responses;

namespace Unitic_BE.Abstracts
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequest model, int accId);
        VnPaymentResponse PaymentExecute(IQueryCollection collections);
    }
}