using Unitic_BE.Entities;

namespace Unitic_BE.Abstracts
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllPayment();
        Task<List<Payment>> GetAllUserPayment(string userId);
        Task<Payment?> GetLastPayment();
        Task<Payment?> GetPayment(string paymentId, string userId);
        Task CreatePayment(Payment payment);
    } 
}