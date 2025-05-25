using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Domain.Entities
{
    public class Coupon 
    {
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal DiscountAmount { get; set; }
        public decimal? MinimumPurchaseAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public List<Product>? ApplicableProducts { get; set; }
        public List<Service>? ApplicableServices { get; set; }
    }
}
