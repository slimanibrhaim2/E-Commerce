using Core.Interfaces;
using Catalogs.Domain.Entities;
using Core.Pagination;

namespace Catalogs.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<PaginatedResult<Product>> GetAllWithDetails(int pageNumber, int pageSize);
    Task<Product?> GetById(Guid id);
    Task<Product?> GetByIdWithDetails(Guid id);
    Task<PaginatedResult<Product>> GetByCategoryWithDetails(Guid categoryId, int pageNumber, int pageSize);
    Task<PaginatedResult<Product>> GetByPriceRange(decimal minPrice, decimal maxPrice, int pageNumber, int pageSize);
    Task<PaginatedResult<Product>> GetLowStockProducts(int threshold, int pageNumber, int pageSize);
    Task<Product> GetByMediaId(Guid mediaId);
    Task<bool> UpdateAsync(Guid id, Product product);
    

    Task<PaginatedResult<Product>> GetProductsByUserIdAsync(Guid userId, int pageNumber, int pageSize);

    Task<PaginatedResult<Product>> GetProductsByNameAsync(string name, int pageNumber, int pageSize);

    Task<PaginatedResult<Product>> GetByIdsAsync(IEnumerable<Guid> ids, int pageNumber, int pageSize);

    Task<Guid?> GetBaseItemIdByProductIdAsync(Guid productId);
    
    // Add reverse lookup method
    Task<Guid?> GetProductIdByBaseItemIdAsync(Guid baseItemId);
} 