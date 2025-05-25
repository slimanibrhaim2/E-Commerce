using Core.Interfaces;
using Catalogs.Domain.Entities;

namespace Catalogs.Domain.Repositories;

public interface IServiceRepository : IRepository<Service>
{
    Task<Service?> GetById(Guid id);
    Task<IEnumerable<Service>> GetByCategory(Guid categoryId);
    Task<IEnumerable<Service>> GetByPriceRange(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Service>> GetAvailableServices();
    Task<Service> GetByMediaId(Guid mediaId);
    Task<IEnumerable<Service>> GetByDurationRange(int minDuration, int maxDuration);
    Task<bool> UpdateAsync(Guid id, Service service);
    Task<IEnumerable<Service>> GetByBrand(Guid brandId);
    Task<bool> AddBrandToServiceAsync(Guid serviceId, Guid brandId);
    Task<bool> RemoveBrandFromServiceAsync(Guid serviceId, Guid brandId);
} 