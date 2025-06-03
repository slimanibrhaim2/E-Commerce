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
    public class PaymentMethodRepository : BaseRepository<PaymentMethod, PaymentMethodDAO>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(ECommerceContext ctx, IMapper<PaymentMethodDAO, PaymentMethod> mapper) : base(ctx, mapper)
        {
        }
    }
}
