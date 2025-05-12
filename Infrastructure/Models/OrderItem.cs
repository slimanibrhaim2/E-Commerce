using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid BaseItemId { get; set; }

    public double Quantity { get; set; }

    public double Price { get; set; }

    public Guid CouponId { get; set; }

    public virtual BaseItem BaseItem { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
