using Shared.Contracts.DTOs;
using System;
using System.Linq;

namespace Shoppings.Application.DTOs
{
    public class CartItemWithDetailsDTO
    {
        public Guid ItemId => ItemDetails?.Id ?? CartItem?.BaseItemId ?? Guid.Empty;
        public string ImageUrl => GetImageUrl();
        public string Name => GetItemName();
        public double Price => GetItemPrice();
        public double TotalPrice => GetItemPrice() * (CartItem?.Quantity ?? 0);
        public int Quantity => CartItem?.Quantity ?? 0;
        
        // Internal use only - will not be serialized due to the computed properties above
        internal CartItemDTO CartItem { get; set; }
        internal ItemDetailsDTO ItemDetails { get; set; }
        
        private string GetItemName()
        {
            return ItemDetails switch
            {
                ProductDetailsDTO product => product.Name,
                ServiceDetailsDTO service => service.Name,
                _ => "عنصر غير معروف"
            };
        }

        private double GetItemPrice()
        {
            return ItemDetails switch
            {
                ProductDetailsDTO product => product.Price,
                ServiceDetailsDTO service => service.Price,
                _ => 0
            };
        }

        private string GetImageUrl()
        {
            if (ItemDetails is ProductDetailsDTO product && product.Media != null && product.Media.Any())
            {
                return product.Media.FirstOrDefault()?.Url;
            }
            else if (ItemDetails is ServiceDetailsDTO service && service.Media != null && service.Media.Any())
            {
                return service.Media.FirstOrDefault()?.Url;
            }
            return null;
        }
    }
} 