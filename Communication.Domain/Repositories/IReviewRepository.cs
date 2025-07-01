using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Communication.Domain.Entities;
using Core.Pagination;
using Core.Interfaces;

namespace Communication.Domain.Repositories
{
    public interface IReviewRepository:IRepository<Review>
    {
        Task<Review> GetByIdAsync(Guid id);
        Task<PaginatedResult<Review>> GetAllAsync(PaginationParameters parameters);
        Task<PaginatedResult<Review>> GetByReviewerIdAsync(Guid reviewerId, PaginationParameters parameters);
        Task<PaginatedResult<Review>> GetByProviderIdAsync(Guid providerId, PaginationParameters parameters);
        Task<PaginatedResult<Review>> GetByOrderIdAsync(Guid orderId, PaginationParameters parameters);
        Task<bool> HasUserReviewedOrderAsync(Guid userId, Guid orderId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task<bool> ExistsAsync(Guid id);
    }
} 