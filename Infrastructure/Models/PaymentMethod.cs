using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PaymentMethod
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
