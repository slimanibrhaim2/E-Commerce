using System;

namespace Catalogs.Application.DTOs
{
    public class FavoriteDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BaseItemId { get; set; }
        public BaseItemDTO BaseItem { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
} 