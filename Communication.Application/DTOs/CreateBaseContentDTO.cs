namespace Communication.Application.DTOs;

public class CreateBaseContentDTO
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
} 