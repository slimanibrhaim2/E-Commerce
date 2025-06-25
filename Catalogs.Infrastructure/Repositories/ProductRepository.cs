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
            .Where(p => p.DeletedAt == null)
            .ToListAsync();
        return products.Select(p => _mapper.Map(p));
    }

    public async Task<Product?> GetById(Guid id)
    {
        var productDao = await _context.Products
            .Include(p => p.BaseItem)
            .Include(p => p.ProductFeatures)
            .FirstOrDefaultAsync(p => p.Id == id);

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
            .Include(p => p.ProductFeatures)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (productDao == null)
            return null;

        return _mapper.Map(productDao);
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
            .Where(p => p.BaseItem.Price >= (double)minPrice && p.BaseItem.Price <= (double)maxPrice);

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedProducts = products.Select(p => _mapper.Map(p)).ToList();
        return PaginatedResult<Product>.Create(mappedProducts, pageNumber, pageSize, totalCount);
    }

    

    public async Task<IEnumerable<Product>> GetLowStockProducts(int threshold)
    {
        var products = await _context.Products
            .Include(p => p.BaseItem)
            .Where(p => p.StockQuantity <= threshold)
            .ToListAsync();

        return products.Select(p => _mapper.Map(p));
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

    public async Task<IEnumerable<Product>> GetProductsByUserIdAsync(Guid userId)
    {
        var products = await _context.Products
            .Include(p => p.BaseItem)
            .Where(p => p.BaseItem.UserId == userId)
            .ToListAsync();
        return products.Select(p => _mapper.Map(p));
    }

    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
    {
        var products = await _context.Products
            .Include(p => p.BaseItem)
            .ToListAsync();
        var results = products
            .Select(p => new { Product = p, Score = Fuzz.Ratio(p.BaseItem.Name, name) })
            .OrderByDescending(x => x.Score)
            .Where(x => x.Score > 60)
            .Select(x => x.Product);
        return results.Select(p => _mapper.Map(p));
    }

    public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var products = await _context.Products
            .Include(p => p.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
            .Include(p => p.ProductFeatures)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
        return products.Select(p => _mapper.Map(p));
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
} 