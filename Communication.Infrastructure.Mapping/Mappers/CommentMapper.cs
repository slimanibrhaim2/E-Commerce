using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Mapping.Mappers;

public class CommentMapper : IMapper<CommentDAO, Comment>
{
    public Comment Map(CommentDAO source)
    {
        if (source == null) return null;
        return new Comment
        {
            Id = source.Id,
            BaseContentId = source.BaseContentId,
            BaseItemId = source.BaseItemId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            // BaseContent and BaseItem can be mapped if needed
        };
    }

    public CommentDAO MapBack(Comment target)
    {
        if (target == null) return null;
        return new CommentDAO
        {
            Id = target.Id,
            BaseContentId = target.BaseContentId,
            BaseItemId = target.BaseItemId,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt,
            // BaseContent and BaseItem can be mapped if needed
        };
    }
} 