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

namespace Users.Infrastructure.Repositories
{
    public class AddressRepository : BaseRepository<Address, AddressDAO>, IAddressRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<AddressDAO, Address> _addressMapper;

        public AddressRepository(ECommerceContext ctx, IMapper<AddressDAO, Address> addressMapper)
            : base(ctx, addressMapper)
        {
            _ctx = ctx;
            _addressMapper = addressMapper;
        }

        public async Task<IEnumerable<Address>> GetAddressesByUserId(Guid userId)
        {
            var daos = await _dbSet.Where(a => a.UserId == userId && a.DeletedAt == null).ToListAsync();
            return daos.Select(a => _addressMapper.Map(a));
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            var addressDaos = await _dbSet.Where(a => a.DeletedAt == null).ToListAsync();
            return addressDaos.Select(a => _addressMapper.Map(a));
        }

        public async Task<Address?> GetByIdAsync(Guid id)
        {
            var dao = await _dbSet.FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null);
            return dao != null ? _addressMapper.Map(dao) : null;
        }

        public async Task<IEnumerable<Address>> FindAsync(Expression<Func<Address, bool>> predicate)
        {
            var addressDaos = await _dbSet.Where(a => a.DeletedAt == null).ToListAsync();
            var domainAddresses = addressDaos.Select(a => _addressMapper.Map(a));
            return domainAddresses.Where(predicate.Compile());
        }

        public async Task AddAsync(Address entity)
        {
            var dao = _addressMapper.MapBack(entity);
            await _dbSet.AddAsync(dao);
        }

        public void Update(Address entity)
        {
            var dao = _addressMapper.MapBack(entity);
            
            // Check if the entity is already being tracked
            var existingEntry = _ctx.ChangeTracker.Entries<AddressDAO>()
                .FirstOrDefault(e => e.Entity.Id == dao.Id);
            
            if (existingEntry != null)
            {
                // Update the existing tracked entity
                existingEntry.CurrentValues.SetValues(dao);
            }
            else
            {
                // Use Attach and set state to Modified to avoid tracking conflicts
                _ctx.Attach(dao);
                _ctx.Entry(dao).State = EntityState.Modified;
            }
        }
    }
}
