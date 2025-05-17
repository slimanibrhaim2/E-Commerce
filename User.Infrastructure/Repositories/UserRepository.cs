// Infrastructure/Persistence/Repositories/UserRepository.cs
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Infrastructure.Mapping;
using Address = Users.Domain.Entities.Address;
using Follower = Users.Domain.Entities.Follower;
using User = Users.Domain.Entities.User;

namespace Users.Infrastructure.Repositories;

public class UserRepository : BaseRepository<UserDAO>, IUserRepository
{
    private readonly ECommerceContext _ctx;
    private readonly IMapper<UserDAO, User> _userMapper;

    public UserRepository(ECommerceContext ctx, IMapper<UserDAO, User> userMapper) : base(ctx)
    {
        _ctx = ctx;
        _userMapper = userMapper;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = await _dbSet.ToListAsync();
        return users.Select(u => _userMapper.Map(u));
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var user = await _dbSet.FindAsync(id);
        return user != null ? _userMapper.Map(user) : null;
    }

    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        var users = await _dbSet.ToListAsync();
        var domainUsers = users.Select(u => _userMapper.Map(u));
        return domainUsers.Where(predicate.Compile());
    }

    public async Task AddAsync(User entity)
    {
        var infraUser = _userMapper.MapBack(entity);
        await _dbSet.AddAsync(infraUser);
    }

    public void Update(User entity)
    {
        var infraUser = _userMapper.MapBack(entity);
        var existingUser = _dbSet.Local.FirstOrDefault(u => u.Id == infraUser.Id);
        if (existingUser != null)
        {
            _ctx.Entry(existingUser).CurrentValues.SetValues(infraUser);
        }
        else
        {
            _dbSet.Update(infraUser);
        }
    }

    public void Remove(User entity)
    {
        var infraUser = _userMapper.MapBack(entity);
        _dbSet.Remove(infraUser);
    }

    public async Task<User?> GetByEmail(string email)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        return user != null ? _userMapper.Map(user) : null;
    }

    public async Task<User?> GetByIdWithDetails(Guid id)
    {
        var user = await _dbSet
            .Include(u => u.Addresses)
            .Include(u => u.Followees)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user != null ? _userMapper.Map(user) : null;
    }

    public async Task<IEnumerable<Follower>> GetFollowersByUserId(Guid userId)
    {
        var followers = await _ctx.Set<FollowerDAO>()
            .Where(f => f.FollowingId == userId)
            .ToListAsync();
        return followers.Select(f => new Follower
        {
            Id = f.Id,
            FollowerId = f.FollowerId,
            FollowingId = f.FollowingId
        });
    }

    public async Task<IEnumerable<Address>> GetAddressesByUserId(Guid userId)
    {
        var addresses = await _ctx.Set<AddressDAO>()
            .Where(a => a.UserId == userId)
            .ToListAsync();
        return addresses.Select(a => new Address
        {
            Id = a.Id,
            UserId = a.UserId,
            Name = a.Name,
            Latitude = a.Latitude ?? 0,
            Longitude = a.Longitude ?? 0
        });
    }
}
