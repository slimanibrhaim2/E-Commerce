using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class DiscountTypeDAO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public virtual ICollection<CouponDAO> Coupons { get; set; } = new List<CouponDAO>();
}
