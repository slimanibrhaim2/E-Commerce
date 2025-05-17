using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmail(string email);

        // used by GetUserByIdQuery
        Task<User?> GetByIdWithDetails(Guid id);

        // used by GetAllFollowersByUserIdQuery
        Task<IEnumerable<Follower>> GetFollowersByUserId(Guid userId);

        // used by GetAddressesByUserIdQuery
        Task<IEnumerable<Address>> GetAddressesByUserId(Guid userId);

        Task<User?> GetByAddressId(Guid addressId);
        Task<User?> GetByFollowerId(Guid followerId);
    }
}
