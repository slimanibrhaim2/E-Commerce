namespace Catalogs.Domain.Entities
{
    public class Brand 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
        public List<Product>? Products { get; set; }
        public List<Service>? Services { get; set; }
    }
}
