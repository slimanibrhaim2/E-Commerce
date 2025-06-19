using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class CategoryMapper : IMapper<CategoryDAO, Category>
{
    public Category Map(CategoryDAO source)
    {
        if (source == null) return null;
        return new Category
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            ParentCategoryId = source.ParentId,
            IsActive = source.IsActive,
            ImageUrl = source.ImageUrl,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt
        };
    }

    public CategoryDAO MapBack(Category target)
    {
        if (target == null) return null;
        return new CategoryDAO
        {
            Id = target.Id,
            Name = target.Name,
            Description = target.Description,
            ParentId = target.ParentCategoryId,
            IsActive = target.IsActive,
            ImageUrl = target.ImageUrl,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt
        };
    }
} 