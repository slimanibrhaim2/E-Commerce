using System;

namespace Catalogs.Application.DTOs
{
    public class ItemIdResponseDTO
    {
        public Guid ItemId { get; set; }
        public string ItemType { get; set; } = string.Empty; // "Product" or "Service"
    }
} 