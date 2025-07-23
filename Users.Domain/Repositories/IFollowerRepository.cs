using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IFollowerRepository : IRepository<Follower>
    {
        Task<IEnumerable<Follower>> GetFollowersByUserId(Guid userId);
        Task<IEnumerable<Follower>> GetFollowingByUserId(Guid userId);
        Task AddAsync(Follower entity);
        Task<Follower?> GetByFollowerAndFollowingId(Guid followerId, Guid followingId);
    }
}
