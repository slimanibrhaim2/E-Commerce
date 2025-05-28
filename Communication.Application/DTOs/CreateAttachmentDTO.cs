namespace Communication.Application.DTOs;

public class CreateAttachmentDTO
{
    public Guid BaseContentId { get; set; }
    public string AttachmentUrl { get; set; } = null!;
    public Guid AttachmentTypeId { get; set; }
} 