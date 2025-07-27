namespace Shoppings.Application.DTOs;

public class MyOrderDTO
{
    public Guid Id { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public double TotalAmount { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string PaymentMethodName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? AddressId { get; set; }
    public ICollection<OrderItemWithDetailsDTO> Items { get; set; } = new List<OrderItemWithDetailsDTO>();
} 