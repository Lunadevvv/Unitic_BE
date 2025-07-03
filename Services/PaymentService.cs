
using Unitic_BE.Abstracts;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.Exceptions;

namespace Unitic_BE.Services
{
   public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<List<Payment>> GetAllPayment()
        {
            return await _paymentRepository.GetAllPayment();
        }

        public async Task<List<Payment>> GetAllUserPayment(string userId)
        {
            return await _paymentRepository.GetAllUserPayment(userId);
        }

        public async Task<Payment> GetPayment(string paymentId, string userId)
        {
            var payment = await _paymentRepository.GetPayment(paymentId, userId);
            if (payment == null)
            {
                throw new ObjectNotFoundException("Payment");
            }
            return payment;
        } 
        public async Task CreatePayment(Payment payment, string userId)
        {
            payment.CreatedDate = DateTime.Now;
            payment.UserId = userId;
            await _paymentRepository.CreatePayment(payment);
        }

        public async Task PayMoney(Payment payment, string userId)
        {
            payment.UserId = userId;
            payment.PaidDate = DateTime.Now;
            await _paymentRepository.CreatePayment(payment);
        }

        public async Task<string> GeneratePaymentId()
            {
                Payment lastPayment = await _paymentRepository.GetLastPayment();
                if (lastPayment == null) return "Payment0001";
                int id = int.Parse(lastPayment.PaymentId.Substring(4)) + 1; // lấy id cuối cùng và cộng thêm 1
                string generatedid = "Payment" + id.ToString("D4");
                return generatedid;
        }

    }
}