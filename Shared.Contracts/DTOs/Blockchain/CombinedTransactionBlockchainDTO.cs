namespace Shared.Contracts.DTOs.Blockchain;

public record CombinedTransactionBlockchainDTO
{
    public OrderBlockchainDTO OrderDetails { get; init; }
    public PaymentBlockchainDTO PaymentDetails { get; init; }
    public DateTime TransactionTimestamp { get; init; }
    public string TransactionType { get; init; } = "ORDER_PAYMENT_COMPLETION";
} 