using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class BaseItemMapper : IMapper<BaseItemDAO, BaseItem>
{
    public BaseItem Map(BaseItemDAO source)
    {
        if (source == null) return null;
        return new BaseItem
        {
            Name = source.Name,
            Description = source.Description,
            Price = (decimal)source.Price,
            IsAvailable = source.IsAvailable,
            CategoryId = source.CategoryId ?? Guid.Empty,
            UserId = source.UserId
            // Media, Favorites, etc. can be mapped if needed
        };
    }

    public BaseItemDAO MapBack(BaseItem target)
    {
        if (target == null) return null;
        return new BaseItemDAO
        {
            Name = target.Name,
            Description = target.Description,
            Price = (double)target.Price,
            IsAvailable = target.IsAvailable,
            CategoryId = target.CategoryId,
            UserId = target.UserId
        };
    }
} 