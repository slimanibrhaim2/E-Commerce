using Core.Interfaces;
using Shoppings.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetByIdWithItemsAsync(Guid id);
        Task<IEnumerable<Order>> GetAllWithItemsAsync();
        Task<IEnumerable<Order>> GetAllByUserIdWithItemsAsync(Guid userId);
    }
}
