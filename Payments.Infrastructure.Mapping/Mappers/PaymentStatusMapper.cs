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
    public class PaymentStatusMapper : IMapper<PaymentStatusDAO, PaymentStatus>
    {
        public PaymentStatus Map(PaymentStatusDAO source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new PaymentStatus
            {
                Id = source.Id,
                Name = source.Name,
                CreatedAt = source.CreatedAt,
                UpdatedAt = source.UpdatedAt,
                DeletedAt = source.DeletedAt
            };
        }

        public PaymentStatusDAO MapBack(PaymentStatus target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            return new PaymentStatusDAO
            {
                Id = target.Id,
                Name = target.Name,
                CreatedAt = target.CreatedAt,
                UpdatedAt = target.UpdatedAt,
                DeletedAt = target.DeletedAt
            };
        }
    }
}
