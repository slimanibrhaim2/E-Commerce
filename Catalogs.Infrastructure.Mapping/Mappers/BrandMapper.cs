using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class BrandMapper : IMapper<BrandDAO, Brand>
{
    public Brand Map(BrandDAO source)
    {
        if (source == null) return null;
        return new Brand
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            // Products and Services can be mapped if needed
        };
    }

    public BrandDAO MapBack(Brand target)
    {
        if (target == null) return null;
        return new BrandDAO
        {
            Id = target.Id,
            Name = target.Name,
            Description = target.Description,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt
        };
    }
} 