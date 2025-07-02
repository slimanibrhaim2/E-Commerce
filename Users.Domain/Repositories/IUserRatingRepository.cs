using System;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IUserRatingRepository
    {
        Task<UserRating> GetByIdAsync(Guid id);
        Task<UserRating> GetByUserIdAsync(Guid userId);
        Task<UserRating> CreateAsync(UserRating userRating);
        Task<UserRating> UpdateAsync(UserRating userRating);
        Task DeleteAsync(Guid id);
    }
} 