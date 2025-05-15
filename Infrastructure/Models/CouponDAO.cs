using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class CouponDAO
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Code { get; set; } = null!;

    public double DiscountAmount { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual UserDAO User { get; set; } = null!;
}
