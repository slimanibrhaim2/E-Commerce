namespace Shared.Contracts.DTOs
{
    public class MediaDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public Guid MediaTypeId { get; set; }
        public Guid ItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 