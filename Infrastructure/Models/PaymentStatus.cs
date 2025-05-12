using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PaymentStatus
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
