namespace Shared.Contracts.DTOs.Blockchain;

public record PaymentBlockchainDTO
{
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public double Amount { get; init; }
    public string PaymentMethod { get; init; }
    public string PaymentStatus { get; init; }
    public DateTime TransactionDate { get; init; }
    public string TransactionHash { get; init; }
    public string? PaymentDetails { get; init; }  // For any additional payment-specific details
} 