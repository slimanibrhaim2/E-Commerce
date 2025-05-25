using Core.Interfaces;
using Catalogs.Domain.Entities;

namespace Catalogs.Domain.Repositories;

public interface IBrandRepository : IRepository<Brand>
{
    Task<IEnumerable<Brand>> GetAllBrandsAsync();
    Task<Guid> AddBrandAsync(Brand brand);
    Task<bool> UpdateBrandAsync(Guid id, Brand brand);
    Task<bool> DeleteBrandAsync(Guid id);
    Task<Brand?> GetBrandByIdAsync(Guid id);
} 