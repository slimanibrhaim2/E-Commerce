namespace Shared.Contracts.DTOs
{
    public class ProductDetailsDTO : ItemDetailsDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<MediaDTO> Media { get; set; }
        public List<ProductFeatureDTO> Features { get; set; }
    }
}