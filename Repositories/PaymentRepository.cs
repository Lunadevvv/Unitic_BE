using Microsoft.EntityFrameworkCore;
using Unitic_BE.Abstracts;
using Unitic_BE.Entities;

namespace Unitic_BE.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllPayment()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<List<Payment>> GetAllUserPayment(string userId)
        {
            return await _context.Payments
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }


        public async Task<Payment?> GetLastPayment()
        {
            return await _context.Payments
                .OrderByDescending(p => p.PaymentId)
                .FirstOrDefaultAsync();
        }
        public async Task<Payment?> GetPayment(string paymentId, string userId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId && p.UserId == userId);
        }

        public async Task CreatePayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePayment(Payment updatedPayment)
        {
            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == updatedPayment.PaymentId);
            if (existingPayment == null)
            {
                throw new Exception("Payment not found");
            }
            // Because payment status will be the only value that change
            if (updatedPayment.Status != null)
                existingPayment.Status = updatedPayment.Status;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetUserByPaymentId(string paymentId)
        {
            return await _context.Payments
                .Where(p => p.PaymentId == paymentId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();
        }
    }

}