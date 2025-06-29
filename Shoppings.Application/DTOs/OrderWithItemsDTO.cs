using System;
using System.Collections.Generic;

namespace Shoppings.Application.DTOs
{
    public class OrderWithItemsDTO
    {
        public Guid Id { get; set; }
        public string StatusName { get; set; } = null!;
        public double TotalAmount { get; set; }
        public List<OrderItemDetailsDTO> Items { get; set; } = new List<OrderItemDetailsDTO>();
    }

    public class OrderItemDetailsDTO
    {
        public Guid ItemId { get; set; }
        public string? ImageUrl { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public double Quantity { get; set; }
    }
} 