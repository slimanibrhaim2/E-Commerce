namespace Catalogs.Application.DTOs;

public class MediaDTO
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public Guid MediaTypeId { get; set; }
    public MediaTypeDTO MediaType { get; set; } // Optional, for navigation
    public Guid BaseItemId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 