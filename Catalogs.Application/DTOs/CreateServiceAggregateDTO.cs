using System;
using System.Collections.Generic;

namespace Catalogs.Application.DTOs
{
    public class CreateServiceAggregateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string ServiceType { get; set; }
        public int Duration { get; set; }
        public bool IsAvailable { get; set; }
        public Guid UserId { get; set; }
        public List<CreateMediaDTO> Media { get; set; } = new();
        public List<CreateFeatureDTO> Features { get; set; } = new();
    }
} 