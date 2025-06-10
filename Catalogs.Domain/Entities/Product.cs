using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Domain.Entities
{
    public class Product : BaseItem
    {
        public Guid Id { get; set; }
        public Guid BaseItemId { get; set; }
        public string SKU { get; set; } = null!;
        public int StockQuantity { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool HasDiscount => DiscountPrice.HasValue;
        public List<Coupon>? ApplicableCoupons { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
