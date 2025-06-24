using Communication.Domain.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication.Domain.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetAllByBaseItemIdAsync(Guid baseItemId);
        Task<IEnumerable<Review>> GetAllByUserIdAsync(Guid userId);
        Task<IEnumerable<Review>> GetAllByOrderIdAsync(Guid orderId);
        Task<Review?> GetByUserAndItemIdAsync(Guid userId, Guid baseItemId);
        Task<bool> HasUserReviewedItemAsync(Guid userId, Guid baseItemId);
        Task<bool> HasUserPurchasedItemAsync(Guid userId, Guid baseItemId, Guid orderId);
    }
} 