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
using FuzzySharp;

namespace Users.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User, UserDAO>, IUserRepository
{
    private readonly ECommerceContext _ctx;
    private readonly IMapper<UserDAO, User> _userMapper;

    public UserRepository(ECommerceContext ctx, IMapper<UserDAO, User> userMapper) : base(ctx, userMapper)
    {
        _ctx = ctx;
        _userMapper = userMapper;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var userDaos = await _dbSet.Where(u => u.DeletedAt == null).ToListAsync();
        return userDaos.Select(u => _userMapper.Map(u));
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var userDao = await _dbSet.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        return userDao != null ? _userMapper.Map(userDao) : null;
    }

    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        var userDaos = await _dbSet.Where(u => u.DeletedAt == null).ToListAsync();
        var domainUsers = userDaos.Select(u => _userMapper.Map(u));
        return domainUsers.Where(predicate.Compile());
    }

    public async Task AddAsync(User entity)
    {
        var userDao = _userMapper.MapBack(entity);
        await _dbSet.AddAsync(userDao);
    }

    public void Update(User entity)
    {
        var userDao = _userMapper.MapBack(entity);
        var existingUser = _dbSet.Local.FirstOrDefault(u => u.Id == userDao.Id);
        if (existingUser != null)
        {
            _ctx.Entry(existingUser).CurrentValues.SetValues(userDao);
        }
        else
        {
            _dbSet.Update(userDao);
        }
    }

    public void Remove(User entity)
    {
        var userDao = _userMapper.MapBack(entity);
        _dbSet.Remove(userDao);
    }

    public async Task<User?> GetByEmail(string email)
    {
        var userDao = await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        return userDao != null ? _userMapper.Map(userDao) : null;
    }

    public async Task<User?> GetByIdWithDetails(Guid id)
    {
        var userDao = await _dbSet
            .Include(u => u.Addresses)
            .Include(u => u.Followees)
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        return userDao != null ? _userMapper.Map(userDao) : null;
    }

    public Task<User?> GetByAddressId(Guid addressId) => Task.FromResult<User?>(null);
    public Task<User?> GetByFollowerId(Guid followerId) => Task.FromResult<User?>(null);

    public async Task<IEnumerable<User>> GetUsersByNameAsync(string name)
    {
        var userDaos = await _dbSet.Where(u => u.DeletedAt == null).ToListAsync();
        var results = userDaos
            .Select(u => new {
                User = _userMapper.Map(u),
                Score = new[] {
                    Fuzz.Ratio(u.FirstName, name),
                    Fuzz.Ratio(u.LastName, name),
                    Fuzz.Ratio((u.FirstName + " " + u.LastName).Trim(), name)
                }.Max()
            })
            .OrderByDescending(x => x.Score)
            .Where(x => x.Score > 60)
            .Select(x => x.User);
        return results;
    }
}
