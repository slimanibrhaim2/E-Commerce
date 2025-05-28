namespace Communication.Application.DTOs;
using System;
using System.Collections.Generic;

public class ConversationDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
} 