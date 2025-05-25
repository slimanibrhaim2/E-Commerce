namespace Catalogs.Application.DTOs;

public class FeatureDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
} 