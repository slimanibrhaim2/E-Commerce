using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Pagination;

namespace Communication.Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepository<Review, ReviewDAO>, IReviewRepository
    {
        public ReviewRepository(ECommerceContext ctx, IMapper<ReviewDAO, Review> mapper)
            : base(ctx, mapper) { }

        public async Task<Review> GetByIdAsync(Guid id)
        {
            var dao = await _dbSet.FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);
            return dao != null ? _mapper.Map(dao) : null;
        }

        public async Task<PaginatedResult<Review>> GetAllAsync(PaginationParameters parameters)
        {
            var query = _dbSet.Where(r => r.DeletedAt == null);
            var totalCount = await query.CountAsync();
            
            var daos = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var reviews = daos.Select(d => _mapper.Map(d)).ToList();
            return PaginatedResult<Review>.Create(
                data: reviews,
                pageNumber: parameters.PageNumber,
                pageSize: parameters.PageSize,
                totalCount: totalCount);
        }

        public async Task<PaginatedResult<Review>> GetByReviewerIdAsync(Guid reviewerId, PaginationParameters parameters)
        {
            var query = _dbSet.Where(r => r.ReviewerId == reviewerId && r.DeletedAt == null);
            var totalCount = await query.CountAsync();
            
            var daos = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var reviews = daos.Select(d => _mapper.Map(d)).ToList();
            return PaginatedResult<Review>.Create(
                data: reviews,
                pageNumber: parameters.PageNumber,
                pageSize: parameters.PageSize,
                totalCount: totalCount);
        }

        public async Task<PaginatedResult<Review>> GetByProviderIdAsync(Guid providerId, PaginationParameters parameters)
        {
            var query = _dbSet.Where(r => r.ProviderId == providerId && r.DeletedAt == null);
            var totalCount = await query.CountAsync();
            
            var daos = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var reviews = daos.Select(d => _mapper.Map(d)).ToList();
            return PaginatedResult<Review>.Create(
                data: reviews,
                pageNumber: parameters.PageNumber,
                pageSize: parameters.PageSize,
                totalCount: totalCount);
        }

        public async Task<PaginatedResult<Review>> GetByOrderIdAsync(Guid orderId, PaginationParameters parameters)
        {
            var query = _dbSet.Where(r => r.OrderId == orderId && r.DeletedAt == null);
            var totalCount = await query.CountAsync();
            
            var daos = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var reviews = daos.Select(d => _mapper.Map(d)).ToList();
            return PaginatedResult<Review>.Create(
                data: reviews,
                pageNumber: parameters.PageNumber,
                pageSize: parameters.PageSize,
                totalCount: totalCount);
        }

        public async Task<bool> HasUserReviewedOrderAsync(Guid userId, Guid orderId)
        {
            return await _dbSet.AnyAsync(r => r.ReviewerId == userId && r.OrderId == orderId && r.DeletedAt == null);
        }

        public async Task AddAsync(Review review)
        {
            var dao = new ReviewDAO
            {
                Id = review.Id,
                ExperienceDescription = review.ExperienceDescription,
                OverallSatisfaction = review.OverallSatisfaction,
                ItemQuality = review.ItemQuality,
                Communication = review.Communication,
                Timeliness = review.Timeliness,
                ValueForMoney = review.ValueForMoney,
                NetPromoterScore = review.NetPromoterScore,
                WillUseAgain = review.WillUseAgain,
                ReviewerId = review.ReviewerId,
                ProviderId = review.ProviderId,
                OrderId = review.OrderId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
            
            await _dbSet.AddAsync(dao);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            var dao = await _dbSet.FirstOrDefaultAsync(r => r.Id == review.Id);
            if (dao != null)
            {
                dao.ExperienceDescription = review.ExperienceDescription;
                dao.OverallSatisfaction = review.OverallSatisfaction;
                dao.ItemQuality = review.ItemQuality;
                dao.Communication = review.Communication;
                dao.Timeliness = review.Timeliness;
                dao.ValueForMoney = review.ValueForMoney;
                dao.NetPromoterScore = review.NetPromoterScore;
                dao.WillUseAgain = review.WillUseAgain;
                dao.ReviewerId = review.ReviewerId;
                dao.ProviderId = review.ProviderId;
                dao.OrderId = review.OrderId;
                dao.UpdatedAt = DateTime.UtcNow;

                _dbSet.Update(dao);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Review review)
        {
            var dao = await _dbSet.FirstOrDefaultAsync(r => r.Id == review.Id);
            if (dao != null)
            {
                dao.DeletedAt = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(r => r.Id == id && r.DeletedAt == null);
        }
    }
} 