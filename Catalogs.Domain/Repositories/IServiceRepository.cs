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
    Task<Service?> GetByIdWithDetails(Guid id);
    Task<PaginatedResult<Service>> GetByCategory(Guid categoryId, int pageNumber, int pageSize);
    Task<PaginatedResult<Service>> GetByPriceRange(decimal minPrice, decimal maxPrice, int pageNumber, int pageSize);
    Task<PaginatedResult<Service>> GetAvailableServices(int pageNumber, int pageSize);
    Task<Service> GetByMediaId(Guid mediaId);
    Task<PaginatedResult<Service>> GetByDurationRange(int minDuration, int maxDuration, int pageNumber, int pageSize);
    Task<bool> UpdateAsync(Guid id, Service service);
    Task<PaginatedResult<Service>> GetByBrand(Guid brandId, int pageNumber, int pageSize);
    Task<bool> AddBrandToServiceAsync(Guid serviceId, Guid brandId);
    Task<bool> RemoveBrandFromServiceAsync(Guid serviceId, Guid brandId);
    Task<PaginatedResult<Service>> GetServicesByUserIdAsync(Guid userId, int pageNumber, int pageSize);
    Task<PaginatedResult<Service>> GetServicesByNameAsync(string name, int pageNumber, int pageSize);
    Task<PaginatedResult<Service>> GetByIdsAsync(IEnumerable<Guid> ids, int pageNumber, int pageSize);
    Task<Guid?> GetBaseItemIdByServiceIdAsync(Guid serviceId);
    Task<Guid?> GetServiceIdByBaseItemIdAsync(Guid baseItemId);
    Task<Service> AddAsync(Service service);
    Task<IEnumerable<Service>> GetAllAsync();
    Task<PaginatedResult<Service>> GetAllWithDetails(int pageNumber, int pageSize);
} 