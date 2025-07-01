using System;

namespace Shared.Contracts.DTOs
{
    public class OrderProviderDTO
    {
        public Guid ProviderId { get; set; }

        public OrderProviderDTO(Guid providerId)
        {
            ProviderId = providerId;
        }
    }
} 