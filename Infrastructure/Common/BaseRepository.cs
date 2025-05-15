using Core;
using Core.Abstraction;
using Core.Interfaces;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly ECommerceContext _ctx;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(ECommerceContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public virtual async Task<T?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public virtual async Task AddAsync(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedAt = DateTime.Now;
                baseEntity.UpdatedAt = DateTime.Now;
            }
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.UpdatedAt = DateTime.Now;
            }
            _dbSet.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.DeletedAt = DateTime.Now;
                _dbSet.Update(entity); // Soft delete
            }
            else
            {
                _dbSet.Remove(entity); // Hard delete
            }
        }
    }
}

