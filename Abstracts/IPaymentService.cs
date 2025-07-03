using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts
{
    public interface IPaymentService
    {
        Task<List<Payment>> GetAllPayment();
        Task<List<Payment>> GetAllUserPayment(string userId);
        Task<Payment> GetPayment(string paymentId, string userId);
        Task CreatePayment(Payment payment, string userId);
        Task PayMoney(Payment payment, string userId);
        Task<string> GeneratePaymentId();
    } 
}