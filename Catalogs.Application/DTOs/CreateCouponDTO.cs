using System;
using System.Collections.Generic;
using Catalogs.Domain.Entities;

namespace Catalogs.Application.DTOs
{
    public class CreateCouponDTO
    {
        public Guid UserId { get; set; }
        public string Code { get; set; } = null!;
        public double DiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Description { get; set; } = null!;
        public decimal? MinimumPurchaseAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? UsageLimit { get; set; }
        public List<Product>? ApplicableProducts { get; set; }
        public List<Service>? ApplicableServices { get; set; }
    }
} 