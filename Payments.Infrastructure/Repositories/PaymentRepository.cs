using Infrastructure.Common;
using Infrastructure.Models;
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
        public PaymentRepository(ECommerceContext ctx, IMapper<PaymentDAO, Payment> mapper) : base(ctx, mapper)
        {
        }
    }
}
