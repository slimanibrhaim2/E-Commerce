using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;

namespace Catalogs.Infrastructure.Repositories
{
    public class MediaTypeRepository : BaseRepository<MediaType, MediaTypeDAO>, IMediaTypeRepository
    {
        public MediaTypeRepository(ECommerceContext context, IMapper<MediaTypeDAO, MediaType> mapper)
            : base(context, mapper)
        {
        }

        // Only implement methods not covered by BaseRepository
        public async Task<bool> UpdateAsync(MediaType mediaType)
        {
            var dao = _ctx.MediaTypes.FirstOrDefault(mt => mt.Id == mediaType.Id && mt.DeletedAt == null);
            if (dao == null) return false;
            dao.Name = mediaType.Name;
            dao.UpdatedAt = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var dao = _ctx.MediaTypes.FirstOrDefault(mt => mt.Id == id && mt.DeletedAt == null);
            if (dao == null) return false;
            dao.DeletedAt = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<Guid> AddAsync(MediaType mediaType)
        {
            var dao = new MediaTypeDAO
            {
                Id = Guid.NewGuid(),
                Name = mediaType.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.MediaTypes.Add(dao);
            await _ctx.SaveChangesAsync();
            return dao.Id;
        }
    }
} 