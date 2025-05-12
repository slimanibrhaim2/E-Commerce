using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class DiscountType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}
