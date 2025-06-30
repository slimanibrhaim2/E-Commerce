using System;

namespace Shared.Contracts.DTOs
{
    public class ItemIdResponseDTO
    {
        public Guid ItemId { get; set; }
        public string ItemType { get; set; } // "Product" or "Service"
    }
} 