namespace Catalogs.Application.DTOs;

public class ServiceFeatureDTO : FeatureDTO
{
    public Guid ServiceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }

} 