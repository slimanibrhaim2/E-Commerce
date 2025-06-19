using System;
using System.Collections.Generic;

namespace Catalogs.Application.DTOs
{
    public class CreateProductAggregateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public List<CreateMediaDTO> Media { get; set; } = new();
        public List<CreateFeatureDTO> Features { get; set; } = new();
        public List<CreateCouponDTO> Coupons { get; set; } = new();
    }
} 