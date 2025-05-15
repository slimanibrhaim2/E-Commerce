using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class OrderStatusDAO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public virtual ICollection<OrderActivityDAO> OrderActivities { get; set; } = new List<OrderActivityDAO>();
}
