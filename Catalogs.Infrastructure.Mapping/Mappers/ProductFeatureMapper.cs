using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class ProductFeatureMapper : IMapper<ProductFeatureDAO, ProductFeature>
{
    public ProductFeature Map(ProductFeatureDAO source)
    {
        if (source == null) return null;
        return new ProductFeature
        {
            Id = source.Id,
            Name = source.Name,
            Value = source.Value,
            BaseItemId = source.ProductId
        };
    }

    public ProductFeatureDAO MapBack(ProductFeature target)
    {
        if (target == null) return null;
        return new ProductFeatureDAO
        {
            Id = target.Id,
            Name = target.Name,
            Value = target.Value,
            ProductId = target.BaseItemId
        };
    }
} 