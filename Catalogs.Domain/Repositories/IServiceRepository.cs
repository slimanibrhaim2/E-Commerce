using Core.Interfaces;
using Catalogs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Pagination;

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
    Task<IEnumerable<Service>> GetServicesByUserIdAsync(Guid userId);
    Task<IEnumerable<Service>> GetServicesByNameAsync(string name);
    Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<Guid> ids);

    Task<Guid?> GetBaseItemIdByServiceIdAsync(Guid serviceId);
    
    // Add reverse lookup method
    Task<Guid?> GetServiceIdByBaseItemIdAsync(Guid baseItemId);

    Task<Service?> GetByIdWithDetails(Guid id);
    Task<Service> AddAsync(Service service);
    Task<PaginatedResult<Service>> GetByPriceRange(decimal minPrice, decimal maxPrice, int pageNumber, int pageSize);

    Task<IEnumerable<Service>> GetAllAsync();
    Task<IEnumerable<Service>> GetAllWithDetails();
} 