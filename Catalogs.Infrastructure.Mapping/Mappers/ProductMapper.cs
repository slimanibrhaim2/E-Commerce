using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class ProductMapper : IMapper<ProductDAO, Product>
{
    public Product Map(ProductDAO source)
    {
        if (source == null) return null;
        return new Product
        {
            Id = source.Id,
            SKU = source.SKU,
            StockQuantity = source.StockQuantity,
            Name = source.BaseItem?.Name,
            Description = source.BaseItem?.Description,
            Price = (decimal)(source.BaseItem?.Price ?? 0),
            CategoryId = source.BaseItem?.CategoryId ?? Guid.Empty,
            IsAvailable = source.BaseItem?.IsAvailable ?? false,
            UserId = source.BaseItem?.UserId ?? Guid.Empty,
            Features = source.ProductFeatures?.Select(f => new ProductFeature
            {
                Id = f.Id,
                Name = f.Name,
                Value = f.Value,
                BaseItemId = source.Id,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList() ?? new List<ProductFeature>(),
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public ProductDAO MapBack(Product target)
    {
        if (target == null) return null;
        return new ProductDAO
        {
            Id = target.Id,
            SKU = target.SKU,
            StockQuantity = target.StockQuantity,
            BaseItem = new BaseItemDAO
            {
                Name = target.Name,
                Description = target.Description,
                Price = (double)target.Price,
                CategoryId = target.CategoryId,
                IsAvailable = target.IsAvailable,
                UserId = target.UserId
            },
            ProductFeatures = target.Features?.Select(f => new ProductFeatureDAO
            {
                Id = f.Id,
                Name = f.Name,
                Value = f.Value,
                ProductId = target.Id,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList() ?? new List<ProductFeatureDAO>(),
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt
        };
    }
} 