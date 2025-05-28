using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Mapping.Mappers;

public class ConversationMapper : IMapper<ConversationDAO, Conversation>
{
    public Conversation Map(ConversationDAO source)
    {
        if (source == null) return null;
        return new Conversation
        {
            Id = source.Id,
            Title = source.Title,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            // ConversationMembers and Messages can be mapped if needed
        };
    }

    public ConversationDAO MapBack(Conversation target)
    {
        if (target == null) return null;
        return new ConversationDAO
        {
            Id = target.Id,
            Title = target.Title,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt,
            // ConversationMembers and Messages can be mapped if needed
        };
    }
} 