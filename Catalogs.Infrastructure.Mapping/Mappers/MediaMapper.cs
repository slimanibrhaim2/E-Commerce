using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class MediaMapper : IMapper<MediaDAO, Media>
{
    public Media Map(MediaDAO source)
    {
        if (source == null) return null;
        return new Media()
        {
            Id = source.Id,
            MediaUrl = source.MediaUrl,
            MediaTypeId = source.MediaTypeId,
            BaseItemId = source.BaseItemId
        };
    }

    public MediaDAO MapBack(Media target)
    {
        if (target == null) return null;
        return new MediaDAO
        {
            Id = target.Id,
            MediaUrl = target.MediaUrl,
            MediaTypeId = target.MediaTypeId,
            BaseItemId = target.BaseItemId,
            // Set CreatedAt, UpdatedAt, DeletedAt as needed
        };
    }
} 