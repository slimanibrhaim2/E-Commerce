using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Coupon
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BaseItemId { get; set; }

    public string Code { get; set; } = null!;

    public Guid DiscountTypeId { get; set; }

    public double DiscountValue { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual BaseItem BaseItem { get; set; } = null!;

    public virtual DiscountType DiscountType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
