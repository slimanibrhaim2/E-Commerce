using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class FavoriteMapper : IMapper<FavoriteDAO, Favorite>
{
    private readonly IMapper<BaseItemDAO, BaseItem> _baseItemMapper;

    public FavoriteMapper(IMapper<BaseItemDAO, BaseItem> baseItemMapper)
    {
        _baseItemMapper = baseItemMapper;
    }

    public Favorite Map(FavoriteDAO source)
    {
        if (source == null) return null;
        return new Favorite
        {
            Id = source.Id,
            UserId = source.UserId,
            BaseItemId = source.BaseItemId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            BaseItem = source.BaseItem != null ? _baseItemMapper.Map(source.BaseItem) : null
        };
    }

    public FavoriteDAO MapBack(Favorite target)
    {
        if (target == null) return null;
        return new FavoriteDAO
        {
            Id = target.Id,
            UserId = target.UserId,
            BaseItemId = target.BaseItemId,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt
        };
    }
}