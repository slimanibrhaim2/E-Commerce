namespace Shoppings.Application.DTOs;

public class MyOrderDTO
{
    public Guid Id { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public double TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? AddressId { get; set; }
    public ICollection<OrderItemWithDetailsDTO> Items { get; set; } = new List<OrderItemWithDetailsDTO>();
} 