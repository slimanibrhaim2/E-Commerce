using Core.Interfaces;
using Catalogs.Domain.Entities;
using Core.Pagination;

namespace Catalogs.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetById(Guid id);
    Task<IEnumerable<Product>> GetByCategory(Guid categoryId);
    Task<IEnumerable<Product>> GetByBrand(Guid brandId);
    Task<PaginatedResult<Product>> GetByPriceRange(decimal minPrice, decimal maxPrice, int pageNumber, int pageSize);
    Task<IEnumerable<Product>> GetLowStockProducts(int threshold);
    Task<Product> GetByMediaId(Guid mediaId);
    Task<bool> UpdateAsync(Guid id, Product product);
    
    // Brand operations
    Task<bool> AddBrandToProductAsync(Guid productId, Guid brandId);
    Task<bool> RemoveBrandFromProductAsync(Guid productId, Guid brandId);

    Task<IEnumerable<Product>> GetProductsByUserIdAsync(Guid userId);

    Task<IEnumerable<Product>> GetProductsByNameAsync(string name);

    Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids);

    Task<Guid?> GetBaseItemIdByProductIdAsync(Guid productId);
} 