namespace Communication.Application.DTOs;

public class CreateMessageDTO
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public Guid BaseContentId { get; set; }
} 