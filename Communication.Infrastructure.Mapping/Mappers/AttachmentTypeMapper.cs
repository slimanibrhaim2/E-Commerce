using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Mapping.Mappers;

public class AttachmentTypeMapper : IMapper<AttachmentTypeDAO, AttachmentType>
{
    public AttachmentType Map(AttachmentTypeDAO source)
    {
        if (source == null) return null;
        return new AttachmentType
        {
            Id = source.Id,
            Name = source.Name,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            // Attachments can be mapped if needed
        };
    }

    public AttachmentTypeDAO MapBack(AttachmentType target)
    {
        if (target == null) return null;
        return new AttachmentTypeDAO
        {
            Id = target.Id,
            Name = target.Name,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt,
            // Attachments can be mapped if needed
        };
    }
} 