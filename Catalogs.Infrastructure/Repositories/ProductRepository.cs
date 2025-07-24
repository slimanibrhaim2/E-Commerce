using Microsoft.EntityFrameworkCore;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using AutoMapper;
using Core.Pagination;
using Catalogs.Infrastructure.Mapping.Mappers;
using FuzzySharp;

namespace Catalogs.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product, ProductDAO>, IProductRepository
{
    private readonly ECommerceContext _context;
    private readonly IMapper<ProductDAO, Product> _mapper;
    private readonly IMapper<CategoryDAO, Category> _categoryMapper;

    public ProductRepository(ECommerceContext context, IMapper<ProductDAO, Product> mapper, IMapper<CategoryDAO, Category> categoryMapper) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
        _categoryMapper = categoryMapper;
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        var products = await _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => p.DeletedAt == null)
            .ToListAsync();
        return products.Select(p => _mapper.Map(p));
    }

    public async Task<Product?> GetById(Guid id)
    {
        var productDao = await _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

        if (productDao == null)
            return null;

        return _mapper.Map(productDao);
    }

    public async Task<Product?> GetByIdWithDetails(Guid id)
    {
        var productDao = await _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.ProductFeatures)
            .Where(p => p.Id == id && p.DeletedAt == null)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return productDao == null ? null : _mapper.Map(productDao);
    }

    public async Task<IEnumerable<Product>> GetByCategory(Guid categoryId)
    {
        var products = await _context.Products
            .Include(p => p.BaseItem)
            .Where(p => p.BaseItem.CategoryId == categoryId)
            .ToListAsync();

        return products.Select(p => _mapper.Map(p));
    }

    public async Task<IEnumerable<Product>> GetByBrand(Guid brandId)
    {
        var products = await _context.Products
            .Include(p => p.BaseItem)
            .Include(p => p.Brands)
            .Where(p => p.Brands.Any(b => b.Id == brandId))
            .ToListAsync();

        return products.Select(p => _mapper.Map(p));
    }

    public async Task<PaginatedResult<Product>> GetByPriceRange(decimal minPrice, decimal maxPrice, int pageNumber, int pageSize)
    {
        var query = _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => p.BaseItem.Price >= (double)minPrice && 
                       p.BaseItem.Price <= (double)maxPrice && 
                       p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt)
            .AsSplitQuery()
            .AsNoTracking();

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedProducts = products.Select(p => _mapper.Map(p)).ToList();
        return PaginatedResult<Product>.Create(mappedProducts, pageNumber, pageSize, totalCount);
    }

    public async Task<PaginatedResult<Product>> GetLowStockProducts(int threshold, int pageNumber, int pageSize)
    {
        var query = _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => p.StockQuantity <= threshold && p.DeletedAt == null)
            .AsSplitQuery()
            .AsNoTracking()
            .OrderBy(p => p.StockQuantity);

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedProducts = products.Select(p => _mapper.Map(p)).ToList();
        return PaginatedResult<Product>.Create(mappedProducts, pageNumber, pageSize, totalCount);
    }

    public async Task<Product> GetByMediaId(Guid mediaId)
    {
        var product = await _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
            .FirstOrDefaultAsync(p => p.BaseItem.ProductMedia.Any(m => m.Id == mediaId));

        if (product == null)
            throw new KeyNotFoundException($"No product found with media ID: {mediaId}");

        return _mapper.Map(product);
    }

    public async Task<bool> UpdateAsync(Guid id, Product product)
    {
        var productDao = await _context.Products
            .Include(p => p.BaseItem)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (productDao == null)
            return false;

        // Update BaseItem properties
        productDao.BaseItem.Name = product.Name;
        productDao.BaseItem.Description = product.Description;
        productDao.BaseItem.Price = (double)product.Price;
        productDao.BaseItem.CategoryId = product.CategoryId;
        productDao.BaseItem.IsAvailable = product.IsAvailable;
        productDao.BaseItem.UpdatedAt = DateTime.UtcNow;

        // Update Product specific properties
        productDao.StockQuantity = product.StockQuantity;
        productDao.SKU = product.SKU;
        productDao.SerialNumber = product.SerialNumber;
        productDao.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // Brand operations
    public async Task<bool> AddBrandToProductAsync(Guid productId, Guid brandId)
    {
        var product = await _context.Products
            .Include(p => p.Brands)
            .FirstOrDefaultAsync(p => p.Id == productId);

        var brand = await _context.Brands.FindAsync(brandId);

        if (product == null || brand == null)
            return false;

        if (product.Brands.Any(b => b.Id == brandId))
            return true; // Brand already associated

        product.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveBrandFromProductAsync(Guid productId, Guid brandId)
    {
        var product = await _context.Products
            .Include(p => p.Brands)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return false;

        var brand = product.Brands.FirstOrDefault(b => b.Id == brandId);
        if (brand == null)
            return true; // Brand not associated

        product.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PaginatedResult<Product>> GetProductsByUserIdAsync(Guid userId, int pageNumber, int pageSize)
    {
        var query = _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => p.BaseItem.UserId == userId && p.DeletedAt == null)
            .AsSplitQuery()
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedProducts = products.Select(p => _mapper.Map(p)).ToList();
        return PaginatedResult<Product>.Create(mappedProducts, pageNumber, pageSize, totalCount);
    }

    public async Task<PaginatedResult<Product>> GetProductsByNameAsync(string name, int pageNumber, int pageSize)
    {
        var query = _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => p.DeletedAt == null && 
                (p.BaseItem.Name.Contains(name) || 
                 EF.Functions.Like(p.BaseItem.Name, $"%{name}%")))
            .AsSplitQuery()
            .AsNoTracking();

        // Get all matching products first to apply fuzzy search
        var allProducts = await query.ToListAsync();
        
        // Apply fuzzy search on all results
        var scoredProducts = allProducts
            .Select(p => new { Product = p, Score = Fuzz.Ratio(p.BaseItem.Name, name) })
            .Where(x => x.Score > 60)
            .OrderByDescending(x => x.Score)
            .ToList();

        var totalCount = scoredProducts.Count;
        
        // Apply pagination after fuzzy search
        var results = scoredProducts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.Product)
            .ToList();

        return PaginatedResult<Product>.Create(
            results.Select(p => _mapper.Map(p)).ToList(), 
            pageNumber, 
            pageSize, 
            totalCount);
    }

    public async Task<PaginatedResult<Product>> GetByIdsAsync(IEnumerable<Guid> ids, int pageNumber, int pageSize)
    {
        var query = _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => ids.Contains(p.Id) && p.DeletedAt == null)
            .AsSplitQuery()
            .AsNoTracking()
            .OrderBy(p => p.CreatedAt);

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedProducts = products.Select(p => _mapper.Map(p)).ToList();
        return PaginatedResult<Product>.Create(mappedProducts, pageNumber, pageSize, totalCount);
    }

    public async Task<Guid?> GetBaseItemIdByProductIdAsync(Guid productId)
    {
        var product = await _context.Products
            .Where(p => p.Id == productId && p.DeletedAt == null)
            .Select(p => p.BaseItemId)
            .FirstOrDefaultAsync();

        return product == Guid.Empty ? null : product;
    }

    public async Task<Guid?> GetProductIdByBaseItemIdAsync(Guid baseItemId)
    {
        var product = await _context.Products
            .Where(p => p.BaseItemId == baseItemId && p.DeletedAt == null)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        return product == Guid.Empty ? null : product;
    }

    public async Task<PaginatedResult<Product>> GetAllWithDetails(int pageNumber, int pageSize)
    {
        var query = _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt)
            .AsSplitQuery()
            .AsNoTracking();

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedProducts = products.Select(p => _mapper.Map(p)).ToList();
        return PaginatedResult<Product>.Create(mappedProducts, pageNumber, pageSize, totalCount);
    }

    public async Task<PaginatedResult<Product>> GetByCategoryWithDetails(Guid categoryId, int pageNumber, int pageSize)
    {
        var query = _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => p.BaseItem.CategoryId == categoryId && p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt)
            .AsSplitQuery()
            .AsNoTracking();

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedProducts = products.Select(p => _mapper.Map(p)).ToList();
        return PaginatedResult<Product>.Create(mappedProducts, pageNumber, pageSize, totalCount);
    }

    public async Task<PaginatedResult<Product>> GetByFiltersWithDetails(
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int pageNumber,
        int pageSize)
    {
        var query = _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
                    .ThenInclude(m => m.MediaType)
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(p => p.ProductFeatures)
            .Where(p => p.DeletedAt == null);

        // Apply category filter if provided
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.BaseItem.CategoryId == categoryId.Value);
        }

        // Apply price range filters if provided
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.BaseItem.Price >= (double)minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.BaseItem.Price <= (double)maxPrice.Value);
        }

        // Order by creation date and optimize query
        query = query
            .OrderByDescending(p => p.CreatedAt)
            .AsSplitQuery()
            .AsNoTracking();

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedProducts = products.Select(p => _mapper.Map(p)).ToList();
        return PaginatedResult<Product>.Create(mappedProducts, pageNumber, pageSize, totalCount);
    }
} 