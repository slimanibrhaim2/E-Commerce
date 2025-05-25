namespace Catalogs.Application.DTOs;

public class BrandDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ProductDTO> Products { get; set; } = new List<ProductDTO>();

    public string LogoUrl { get; set; }

    public bool IsActive { get; set; }

    public List<Guid>? ProductIds { get; set; }

    public List<Guid>? ServiceIds { get; set; }
} 