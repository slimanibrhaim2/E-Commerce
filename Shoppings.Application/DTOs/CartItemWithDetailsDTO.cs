using Shared.Contracts.DTOs;
using System;

namespace Shoppings.Application.DTOs
{
    public class CartItemWithDetailsDTO
    {
        public CartItemDTO CartItem { get; set; }
        public ItemDetailsDTO ItemDetails { get; set; }
        
        // Helper properties for easy access
        public string ItemName => (ItemDetails as ProductDetailsDTO)?.Name ?? (ItemDetails as ServiceDetailsDTO)?.Name;
        public string ItemDescription => (ItemDetails as ProductDetailsDTO)?.Description ?? (ItemDetails as ServiceDetailsDTO)?.Description;
        public decimal? ItemPrice => (ItemDetails as ProductDetailsDTO)?.Price ?? (ItemDetails as ServiceDetailsDTO)?.Price;
        public bool? IsAvailable => (ItemDetails as ProductDetailsDTO)?.IsAvailable ?? (ItemDetails as ServiceDetailsDTO)?.IsAvailable;
        public string ItemType => ItemDetails is ProductDetailsDTO ? "Product" : ItemDetails is ServiceDetailsDTO ? "Service" : "Unknown";
        
        // Get the original Item ID (Product ID or Service ID) from ItemDetails
        public Guid? ItemId => ItemDetails?.Id;
        
        // Calculated property
        public decimal? TotalPrice => ItemPrice.HasValue ? ItemPrice.Value * CartItem.Quantity : null;
    }
} 