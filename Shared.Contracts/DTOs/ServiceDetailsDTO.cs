namespace Shared.Contracts.DTOs
{
    public class ServiceDetailsDTO : ItemDetailsDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Guid CategoryId { get; set; }
        public string ServiceType { get; set; }
        public int Duration { get; set; }
        public bool IsAvailable { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<MediaDTO> Media { get; set; }
    }
}