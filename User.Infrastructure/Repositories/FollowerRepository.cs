using Infrastructure.Common;
using Infrastructure.Models;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Infrastructure.Mapping.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Core.Result;

namespace Users.Infrastructure.Repositories
{
    public class FollowerRepository : BaseRepository<Follower, FollowerDAO>, IFollowerRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<FollowerDAO, Follower> _followerMapper;

        public FollowerRepository(ECommerceContext ctx, IMapper<FollowerDAO, Follower> followerMapper)
            : base(ctx, followerMapper)
        {
            _ctx = ctx;
            _followerMapper = followerMapper;
        }

        public async Task<IEnumerable<Follower>> GetFollowersByUserId(Guid userId)
        {
            var daos = await _dbSet
                .Include(f => f.Follower) // Include the follower user details
                .Where(f => f.FollowingId == userId && f.DeletedAt == null)
                .ToListAsync();
            return daos.Select(f => _followerMapper.Map(f));
        }
        public async Task<IEnumerable<Follower>> GetFollowingByUserId(Guid userId)
        {
            var daos = await _dbSet
                .Include(f => f.Following) // Include the following user details
                .Where(f => f.FollowerId == userId && f.DeletedAt == null)
                .ToListAsync();
            return daos.Select(f => _followerMapper.Map(f));
        }


        public async Task<Follower?> GetByFollowerAndFollowingId(Guid followerId, Guid followingId)
        {
            var dao = await _dbSet.FirstOrDefaultAsync(f => 
                f.FollowerId == followerId && 
                f.FollowingId == followingId && 
                f.DeletedAt == null);
            return dao == null ? null : _followerMapper.Map(dao);
        }

        public async Task AddAsync(Follower entity)
        {
            FollowerDAO dao = _followerMapper.MapBack(entity);
            await _dbSet.AddAsync(dao);
        }

    }
}
