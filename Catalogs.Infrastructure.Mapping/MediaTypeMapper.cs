using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class MediaTypeMapper : IMapper<MediaTypeDAO, MediaType>
{
    public MediaType Map(MediaTypeDAO source)
    {
        if (source == null) return null;
        return new MediaType
        {
            Id = source.Id,
            Name = source.Name,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt
        };
    }

    public MediaTypeDAO MapBack(MediaType target)
    {
        if (target == null) return null;
        return new MediaTypeDAO
        {
            Id = target.Id,
            Name = target.Name,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt
        };
    }
} 