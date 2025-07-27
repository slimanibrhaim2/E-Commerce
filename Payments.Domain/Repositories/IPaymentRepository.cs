using Core.Interfaces;
using Payments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Domain.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task UpdateFields(Guid id, Dictionary<string, object> fields);
        Task<Payment?> GetByOrderIdAsync(Guid orderId);
    }
}
