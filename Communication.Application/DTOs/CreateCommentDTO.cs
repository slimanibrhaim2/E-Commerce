namespace Communication.Application.DTOs;

public class CreateCommentDTO
{
    public Guid BaseContentId { get; set; }
    public Guid ItemId { get; set; }
} 