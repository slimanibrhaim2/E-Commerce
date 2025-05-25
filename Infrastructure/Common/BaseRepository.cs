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
            var daos = await _dbSet.ToListAsync();
            return daos.Select(d => _mapper.Map(d));
        }

        public virtual async Task<TDomain?> GetByIdAsync(Guid id)
        {
            var dao = await _dbSet.FindAsync(id);
            return dao != null ? _mapper.Map(dao) : null;
        }

        public virtual async Task<IEnumerable<TDomain>> FindAsync(Expression<Func<TDomain, bool>> predicate)
        {
            var daos = await _dbSet.ToListAsync();
            var domain = daos.Select(d => _mapper.Map(d));
            return domain.Where(predicate.Compile());
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
            _dbSet.Remove(dao);
        }
    }
}

