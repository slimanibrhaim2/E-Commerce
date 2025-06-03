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
    public class PaymentMethodMapper : IMapper<PaymentMethodDAO, PaymentMethod>
    {
        public PaymentMethod Map(PaymentMethodDAO source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new PaymentMethod
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                IsActive = source.IsActive,
                CreatedAt = source.CreatedAt,
                UpdatedAt = source.UpdatedAt,
                DeletedAt = source.DeletedAt
            };
        }

        public PaymentMethodDAO MapBack(PaymentMethod target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            return new PaymentMethodDAO
            {
                Id = target.Id,
                Name = target.Name,
                Description = target.Description,
                IsActive = target.IsActive,
                CreatedAt = target.CreatedAt,
                UpdatedAt = target.UpdatedAt,
                DeletedAt = target.DeletedAt
            };
        }
    }
}
