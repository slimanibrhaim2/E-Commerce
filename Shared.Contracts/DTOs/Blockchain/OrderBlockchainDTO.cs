namespace Shared.Contracts.DTOs.Blockchain;

public record OrderBlockchainDTO
{
    public  Guid OrderId { get; init; }
    public  DateTime OrderDate { get; init; }
    public  string OrderStatus { get; init; }
    public  double TotalAmount { get; init; }
    public  string ShippingAddress { get; init; }
    public  List<OrderItemBlockchainDTO> Items { get; init; }
    public  UserBlockchainDTO Seller { get; init; }
    public  UserBlockchainDTO Customer { get; init; }
}

public record OrderItemBlockchainDTO
{
    public  Guid ItemId { get; init; }
    public  string ItemName { get; init; }
    public  string? SerialNumber { get; init; }
    public  double Quantity { get; init; }
    public  double UnitPrice { get; init; }
    public  double TotalPrice { get; init; }
}



