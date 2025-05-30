namespace Catalogs.Domain.Entities;

public class ServiceFeature 
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public Service? Service { get; set; }
} 