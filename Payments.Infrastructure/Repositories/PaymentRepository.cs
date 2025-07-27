using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Repositories
{
    public class PaymentRepository : BaseRepository<Payment, PaymentDAO>, IPaymentRepository
    {
        private readonly ECommerceContext _context;

        public PaymentRepository(ECommerceContext context, IMapper<PaymentDAO, Payment> mapper)
            : base(context, mapper)
        {
            _context = context;
        }

        public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
        {
            var paymentDAO = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.DeletedAt == null);
            
            return paymentDAO != null ? _mapper.Map(paymentDAO) : null;
        }

        public async Task UpdateFields(Guid id, Dictionary<string, object> fields)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return;

            if (fields.ContainsKey(nameof(Payment.StatusId)))
                payment.StatusId = (Guid)fields[nameof(Payment.StatusId)];
            
            if (fields.ContainsKey(nameof(Payment.UpdatedAt)))
                payment.UpdatedAt = (DateTime)fields[nameof(Payment.UpdatedAt)];
        }
    }
}
