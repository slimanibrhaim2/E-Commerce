namespace Communication.Application.DTOs;

public class CreateConversationMemberDTO
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
} 