using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Mapping.Mappers;

public class MessageMapper : IMapper<MessageDAO, Message>
{
    public Message Map(MessageDAO source)
    {
        if (source == null) return null;
        return new Message
        {
            Id = source.Id,
            ConversationId = source.ConversationId,
            SenderId = source.SenderId,
            BaseContentId = source.BaseContentId,
            IsRead = source.IsRead,
            ReadAt = source.ReadAt,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            // BaseContent and Conversation can be mapped if needed
        };
    }

    public MessageDAO MapBack(Message target)
    {
        if (target == null) return null;
        return new MessageDAO
        {
            Id = target.Id,
            ConversationId = target.ConversationId,
            SenderId = target.SenderId,
            BaseContentId = target.BaseContentId,
            IsRead = target.IsRead,
            ReadAt = target.ReadAt,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt,
            // BaseContent and Conversation can be mapped if needed
        };
    }
} 