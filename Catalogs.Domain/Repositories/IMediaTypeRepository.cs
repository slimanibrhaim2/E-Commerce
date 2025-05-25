using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalogs.Domain.Entities;
using Core.Interfaces;

namespace Catalogs.Domain.Repositories
{
    public interface IMediaTypeRepository : IRepository<MediaType>
    {
        Task<IEnumerable<MediaType>> GetAllAsync();
        Task<MediaType?> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(MediaType mediaType);
        Task<bool> UpdateAsync(MediaType mediaType);
        Task<bool> DeleteAsync(Guid id);
    }
} 