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
            BaseItemId = source.BaseItemId,
            Price = (source.BaseItem?.Price ?? 0),
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
            Media = source.BaseItem?.ProductMedia?.Select(m => new Media
            {
                Id = m.Id,
                MediaUrl = m.MediaUrl,
                MediaTypeId = m.MediaTypeId,
                BaseItemId = m.BaseItemId,
                MediaType = m.MediaType != null ? new MediaType
                {
                    Id = m.MediaType.Id,
                    Name = m.MediaType.Name,
                    CreatedAt = m.MediaType.CreatedAt,
                    UpdatedAt = m.MediaType.UpdatedAt
                } : null,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList() ?? new List<Media>(),
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public ProductDAO MapBack(Product target)
    {
        if (target == null) return null;
        var productDao = new ProductDAO
        {
            Id = target.Id,
            SKU = target.SKU,
            StockQuantity = target.StockQuantity,
            BaseItemId = target.BaseItemId,
            BaseItem = new BaseItemDAO
            {
                Id = target.BaseItemId,
                Name = target.Name,
                Description = target.Description,
                Price = (double)target.Price,
                CategoryId = target.CategoryId,
                IsAvailable = target.IsAvailable,
                UserId = target.UserId,
                ProductMedia = target.Media?.Select(m => new MediaDAO
                {
                    Id = m.Id,
                    MediaUrl = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    BaseItemId = target.BaseItemId,
                    MediaType = m.MediaType != null ? new MediaTypeDAO
                    {
                        Id = m.MediaType.Id,
                        Name = m.MediaType.Name,
                        CreatedAt = m.MediaType.CreatedAt,
                        UpdatedAt = m.MediaType.UpdatedAt
                    } : null,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList() ?? new List<MediaDAO>()
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
        return productDao;
    }
} 