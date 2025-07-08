using Unitic_BE.DTOs.Requests;
using Unitic_BE.DTOs.Responses;

namespace Unitic_BE.Abstracts
{
    public interface IVnPayService
    {
        string CreatePaymentUrl( VnPaymentRequest model);
        VnPaymentResponse GetPaymentResult(IQueryCollection collections);
    }
}