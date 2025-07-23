using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IUserRatingRepository : IRepository<UserRating>
    {
        Task<UserRating?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<UserRating>> GetByUserIdsAsync(IEnumerable<Guid> userIds);
    }
} 