using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class OrderActivityDAO
{
    public Guid Id { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<OrderDAO> Orders { get; set; } = new List<OrderDAO>();
}
