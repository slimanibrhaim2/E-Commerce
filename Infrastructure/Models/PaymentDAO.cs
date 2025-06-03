using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class PaymentDAO
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }

    public Guid PaymentMethodId { get; set; }

    public Guid StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual OrderDAO Order { get; set; } = null!;

    public virtual PaymentMethodDAO PaymentMethod { get; set; } = null!;

    public virtual PaymentStatusDAO Status { get; set; } = null!;
}
