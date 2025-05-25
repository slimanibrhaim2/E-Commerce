namespace Catalogs.Application.DTOs;

public class CategoryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CategoryDTO> SubCategories { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
} 