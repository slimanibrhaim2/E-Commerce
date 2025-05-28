namespace Communication.Application.DTOs;
using System;

public class MessageDTO
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public Guid BaseContentId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public BaseContentDTO BaseContent { get; set; } = null!;
    public ConversationDTO Conversation { get; set; } = null!;
} 