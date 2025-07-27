namespace Shared.Contracts.DTOs;

public record PaymentMethodInfoDTO
{
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public string PaymentMethodName { get; init; } = string.Empty;
    public double Amount { get; init; }
    public string PaymentStatus { get; init; } = string.Empty;
} 