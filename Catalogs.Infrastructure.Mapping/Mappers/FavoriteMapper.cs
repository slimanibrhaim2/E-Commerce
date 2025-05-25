using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class FavoriteMapper : IMapper<FavoriteDAO, Favorite>
{
    public Favorite Map(FavoriteDAO source)
    {
        if (source == null) return null;
        return new Favorite
        {
            Id = source.Id,
            UserId = source.UserId,
            BaseItemId = source.ProductId, // ProductId in DAO maps to BaseItemId in domain
            AddedAt = source.CreatedAt
            // BaseItem can be mapped if needed
        };
    }

    public FavoriteDAO MapBack(Favorite target)
    {
        if (target == null) return null;
        return new FavoriteDAO
        {
            Id = target.Id,
            UserId = target.UserId,
            ProductId = target.BaseItemId, // BaseItemId in domain maps to ProductId in DAO
            CreatedAt = target.AddedAt,
            UpdatedAt = target.AddedAt // Or set as needed
            // Navigation properties can be mapped if needed
        };
    }
} 