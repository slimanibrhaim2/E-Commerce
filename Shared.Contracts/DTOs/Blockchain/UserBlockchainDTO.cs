namespace Shared.Contracts.DTOs.Blockchain;

public record UserBlockchainDTO
{
    public string Username { get; init; }
    public string PhoneNumber { get; init; }
} 