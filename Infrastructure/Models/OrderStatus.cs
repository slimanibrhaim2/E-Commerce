using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class OrderStatus
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<OrderActivity> OrderActivities { get; set; } = new List<OrderActivity>();
}
