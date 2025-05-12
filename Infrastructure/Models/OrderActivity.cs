using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class OrderActivity
{
    public Guid Id { get; set; }

    public Guid OrderStatusId { get; set; }

    public DateTime BecomeAt { get; set; }

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
