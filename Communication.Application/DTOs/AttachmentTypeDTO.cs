namespace Communication.Application.DTOs;
using System;
using System.Collections.Generic;

public class AttachmentTypeDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
} 