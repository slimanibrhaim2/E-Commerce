namespace Communication.Application.DTOs;
using System;

public class CommentDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid BaseContentId { get; set; }
    public Guid BaseItemId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public BaseContentDTO BaseContent { get; set; } = null!;
} 