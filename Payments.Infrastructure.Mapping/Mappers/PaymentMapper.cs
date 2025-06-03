using Infrastructure.Common;
using Infrastructure.Models;
using Payments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Mapping.Mappers
{
    public class PaymentMapper : IMapper<PaymentDAO, Payment>
    {
        public Payment Map(PaymentDAO source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new Payment
            {
                Id = source.Id,
                OrderId = source.OrderId,
                Amount = source.Amount,
                PaymentMethodId = source.PaymentMethodId,
                StatusId = source.StatusId,
                CreatedAt = source.CreatedAt,
                UpdatedAt = source.UpdatedAt,
                DeletedAt = source.DeletedAt
            };
        }

        public PaymentDAO MapBack(Payment target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            return new PaymentDAO
            {
                Id = target.Id,
                OrderId = target.OrderId,
                Amount = target.Amount,
                PaymentMethodId = target.PaymentMethodId,
                StatusId = target.StatusId,
                CreatedAt = target.CreatedAt,
                UpdatedAt = target.UpdatedAt,
                DeletedAt = target.DeletedAt
            };
        }
    }
}
