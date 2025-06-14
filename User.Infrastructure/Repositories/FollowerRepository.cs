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
            var daos = await _dbSet.Where(f => f.FollowingId == userId && f.DeletedAt == null).ToListAsync();
            return daos.Select(f => _followerMapper.Map(f));
        }

        public async Task<User?> GetByFollowerId(Guid followerId)
        {
            var followerDao = await _dbSet.Include(f => f.Follower).FirstOrDefaultAsync(f => f.FollowerId == followerId && f.DeletedAt == null);
            if (followerDao == null)
                return null;
            // Use a UserMapper to map UserDAO to User
            var userMapper = new Users.Infrastructure.Mapping.Mappers.UserMapper();
            return userMapper.Map(followerDao.Follower);
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

        // The rest of the CRUD methods are handled by the base class
    }
}
