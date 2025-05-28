using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Mapping.Mappers;

public class BaseContentMapper : IMapper<BaseContentDAO, BaseContent>
{
    public BaseContent Map(BaseContentDAO source)
    {
        if (source == null) return null;
        return new BaseContent
        {
            Id = source.Id,
            UserId = source.UserId,
            Title = source.Title,
            Description = source.Description,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            // Attachments can be mapped if needed
        };
    }

    public BaseContentDAO MapBack(BaseContent target)
    {
        if (target == null) return null;
        return new BaseContentDAO
        {
            Id = target.Id,
            UserId = target.UserId,
            Title = target.Title,
            Description = target.Description,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt,
            // Attachments can be mapped if needed
        };
    }
} 