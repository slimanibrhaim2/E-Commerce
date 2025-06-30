using System;
using System.Collections.Generic;

namespace Shoppings.Application.DTOs;

public class SellerOrderDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }  // ID of the user who placed the order
    public string OrderStatus { get; set; } = string.Empty;
    public double TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? AddressId { get; set; }
    public List<OrderItemWithDetailsDTO> Items { get; set; } = new();
} 