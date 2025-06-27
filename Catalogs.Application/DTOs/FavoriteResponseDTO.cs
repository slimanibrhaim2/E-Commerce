using System;

namespace Catalogs.Application.DTOs
{
    public class FavoriteResponseDTO
    {
        public int Quantity { get; set; }
        public BaseItemResponseDTO BaseItem { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BaseItemResponseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }
} 