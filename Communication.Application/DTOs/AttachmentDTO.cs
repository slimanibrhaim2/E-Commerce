namespace Communication.Application.DTOs;
using System;

public class AttachmentDTO
{
    public Guid Id { get; set; }
    public Guid BaseContentId { get; set; }
    public string AttachmentUrl { get; set; } = null!;
    public Guid AttachmentTypeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
} 