using Infrastructure.Common;
using Infrastructure.Models;
using Users.Domain.Entities;

namespace Users.Infrastructure.Mapping.Mappers;

public class NotificationMapper : BaseMapper<NotificationDAO, Notification>
{
    public override Notification Map(NotificationDAO source)
    {
        return SafeMap(source, s => new Notification
        {
            Id = s.Id,
            UserId = s.UserId,
            NotificationContenet = s.Message, // Map Message to NotificationContenet
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            DeletedAt = s.DeletedAt
        });
    }

    public override NotificationDAO MapBack(Notification target)
    {
        return SafeMapBack(target, t => new NotificationDAO
        {
            Id = t.Id,
            UserId = t.UserId,
            Message = t.NotificationContenet, // Map NotificationContenet to Message
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            DeletedAt = t.DeletedAt
        });
    }
} 