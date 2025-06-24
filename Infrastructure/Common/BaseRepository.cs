using Core;
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
    public class BaseRepository<TDomain, TDao> : IRepository<TDomain>
        where TDomain : class
        where TDao : class
    {
        protected readonly ECommerceContext _ctx;
        protected readonly DbSet<TDao> _dbSet;
        protected readonly IMapper<TDao, TDomain> _mapper;

        protected BaseRepository(ECommerceContext ctx, IMapper<TDao, TDomain> mapper)
        {
            _ctx = ctx;
            _dbSet = ctx.Set<TDao>();
            _mapper = mapper;
        }

        public virtual async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            // Check if the entity has a DeletedAt property
            var deletedAtProperty = typeof(TDao).GetProperty("DeletedAt");
            if (deletedAtProperty != null && deletedAtProperty.PropertyType == typeof(DateTime?))
            {
                // Filter out deleted entities
                var daos = await _dbSet.Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null).ToListAsync();
                return daos.Select(d => _mapper.Map(d));
            }
            else
            {
                // Fall back to all entities if no DeletedAt property
                var daos = await _dbSet.ToListAsync();
                return daos.Select(d => _mapper.Map(d));
            }
        }

        public virtual async Task<TDomain?> GetByIdAsync(Guid id)
        {
            // Check if the entity has a DeletedAt property
            var deletedAtProperty = typeof(TDao).GetProperty("DeletedAt");
            if (deletedAtProperty != null && deletedAtProperty.PropertyType == typeof(DateTime?))
            {
                // Filter out deleted entities using a simpler approach
                var dao = await _dbSet.FirstOrDefaultAsync(e => 
                    EF.Property<Guid>(e, "Id") == id &&
                    EF.Property<DateTime?>(e, "DeletedAt") == null);
                return dao != null ? _mapper.Map(dao) : null;
            }
            else
            {
                // Fall back to FindAsync for entities without DeletedAt
                var dao = await _dbSet.FindAsync(id);
                return dao != null ? _mapper.Map(dao) : null;
            }
        }

        public virtual async Task<IEnumerable<TDomain>> FindAsync(Expression<Func<TDomain, bool>> predicate)
        {
            // Check if the entity has a DeletedAt property
            var deletedAtProperty = typeof(TDao).GetProperty("DeletedAt");
            if (deletedAtProperty != null && deletedAtProperty.PropertyType == typeof(DateTime?))
            {
                // Filter out deleted entities
                var daos = await _dbSet.Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null).ToListAsync();
                var domain = daos.Select(d => _mapper.Map(d));
                return domain.Where(predicate.Compile());
            }
            else
            {
                // Fall back to all entities if no DeletedAt property
                var daos = await _dbSet.ToListAsync();
                var domain = daos.Select(d => _mapper.Map(d));
                return domain.Where(predicate.Compile());
            }
        }

        public virtual async Task AddAsync(TDomain entity)
        {
            var dao = _mapper.MapBack(entity);
            await _dbSet.AddAsync(dao);
        }

        public virtual void Update(TDomain entity)
        {
            var dao = _mapper.MapBack(entity);
            _dbSet.Update(dao);
        }

        public virtual void Remove(TDomain entity)
        {
            var dao = _mapper.MapBack(entity);
            
            // Check if the entity has a DeletedAt property for soft delete
            var deletedAtProperty = typeof(TDao).GetProperty("DeletedAt");
            if (deletedAtProperty != null && deletedAtProperty.PropertyType == typeof(DateTime?))
            {
                // Soft delete by setting DeletedAt
                deletedAtProperty.SetValue(dao, DateTime.UtcNow);
                
                // Check if the entity is already being tracked
                var idProperty = typeof(TDao).GetProperty("Id");
                var daoId = idProperty?.GetValue(dao);
                var existingEntry = _ctx.ChangeTracker.Entries<TDao>()
                    .FirstOrDefault(e => idProperty?.GetValue(e.Entity).Equals(daoId) == true);
                
                if (existingEntry != null)
                {
                    // Update the existing tracked entity
                    existingEntry.CurrentValues.SetValues(dao);
                }
                else
                {
                    // Attach and mark as modified
                    _ctx.Attach(dao);
                    _ctx.Entry(dao).State = EntityState.Modified;
                }
            }
            else
            {
                // Fall back to hard delete if no DeletedAt property
                _dbSet.Remove(dao);
            }
        }

        public virtual void HardDelete(TDomain entity)
        {
            var dao = _mapper.MapBack(entity);
            _dbSet.Remove(dao);
        }
    }
}

