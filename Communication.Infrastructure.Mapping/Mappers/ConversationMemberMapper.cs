using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Mapping.Mappers;

public class ConversationMemberMapper : IMapper<ConversationMemberDAO, ConversationMember>
{
    public ConversationMember Map(ConversationMemberDAO source)
    {
        if (source == null) return null;
        return new ConversationMember
        {
            Id = source.Id,
            ConversationId = source.ConversationId,
            UserId = source.UserId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            // Conversation can be mapped if needed
        };
    }

    public ConversationMemberDAO MapBack(ConversationMember target)
    {
        if (target == null) return null;
        return new ConversationMemberDAO
        {
            Id = target.Id,
            ConversationId = target.ConversationId,
            UserId = target.UserId,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt,
            // Conversation can be mapped if needed
        };
    }
} 