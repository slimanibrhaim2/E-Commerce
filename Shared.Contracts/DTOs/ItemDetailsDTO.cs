using System;

namespace Shared.Contracts.DTOs
{
    public abstract class ItemDetailsDTO
    {
        // Common property that all item details must have
        public Guid Id { get; set; }
        
        // Abstract marker class for polymorphic returns
        // ProductDetailsDTO and ServiceDetailsDTO will inherit from this
    }
} 