using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid MethodId { get; set; }

    public Guid PaymentStatusId { get; set; }

    public string TransactionId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual PaymentMethod Method { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual PaymentStatus PaymentStatus { get; set; } = null!;
}
