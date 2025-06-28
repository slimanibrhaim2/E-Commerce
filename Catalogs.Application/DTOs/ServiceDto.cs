namespace Catalogs.Application.DTOs;

public class ServiceDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid CategoryId { get; set; }
    public string ServiceType { get; set; }
    public int Duration { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFavorite { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<MediaDTO> Media { get; set; }
    public List<ServiceFeatureDTO> Features { get; set; }
    public string? CategoryName { get; internal set; }
} 