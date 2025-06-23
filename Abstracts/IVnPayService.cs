using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, int accId);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}