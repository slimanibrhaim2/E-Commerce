using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PaymentStatusDAO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public virtual ICollection<PaymentDAO> Payments { get; set; } = new List<PaymentDAO>();
}
