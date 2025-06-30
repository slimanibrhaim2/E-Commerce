using Shared.Contracts.DTOs;
using System;
using System.Linq;

namespace Shoppings.Application.DTOs
{
    public class OrderItemWithDetailsDTO
    {
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        
        // Internal use only - will not be serialized due to the computed properties above
        internal OrderItemDTO OrderItem { get; set; }
        internal ItemDetailsDTO ItemDetails { get; set; }
        
        private string GetItemName()
        {
            if (ItemDetails == null)
                return "عنصر غير معروف";

            return ItemDetails switch
            {
                ProductDetailsDTO product => product.Name,
                ServiceDetailsDTO service => service.Name,
                _ => "عنصر غير معروف"
            };
        }

        private double GetItemPrice()
        {
            if (ItemDetails == null)
                return OrderItem?.Price ?? 0;

            return ItemDetails switch
            {
                ProductDetailsDTO product => product.Price,
                ServiceDetailsDTO service => service.Price,
                _ => OrderItem?.Price ?? 0 // For order items, we can fallback to the stored price
            };
        }

        private string GetImageUrl()
        {
            if (ItemDetails == null)
                return string.Empty;

            if (ItemDetails is ProductDetailsDTO product && product.Media != null && product.Media.Any())
            {
                return product.Media.FirstOrDefault()?.Url ?? string.Empty;
            }
            else if (ItemDetails is ServiceDetailsDTO service && service.Media != null && service.Media.Any())
            {
                return service.Media.FirstOrDefault()?.Url ?? string.Empty;
            }
            return string.Empty;
        }
    }
} 