using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class OrderItemDAO
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid BaseItemId { get; set; }

    public double Quantity { get; set; }

    public double Price { get; set; }

    public Guid CouponId { get; set; }
     public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public virtual BaseItemDAO BaseItem { get; set; } = null!;

    public virtual OrderDAO Order { get; set; } = null!;
}
