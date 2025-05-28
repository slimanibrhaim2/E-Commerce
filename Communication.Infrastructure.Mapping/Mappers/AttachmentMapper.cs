using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Mapping.Mappers;

public class AttachmentMapper : IMapper<AttachmentDAO, Attachment>
{
    public Attachment Map(AttachmentDAO source)
    {
        if (source == null) return null;
        return new Attachment
        {
            Id = source.Id,
            BaseContentId = source.BaseContentId,
            AttachmentUrl = source.AttachmentUrl,
            AttachmentTypeId = source.AttachmentTypeId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            // BaseContent can be mapped if needed
        };
    }

    public AttachmentDAO MapBack(Attachment target)
    {
        if (target == null) return null;
        return new AttachmentDAO
        {
            Id = target.Id,
            BaseContentId = target.BaseContentId,
            AttachmentUrl = target.AttachmentUrl,
            AttachmentTypeId = target.AttachmentTypeId,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt,
            // BaseContent can be mapped if needed
        };
    }
} 