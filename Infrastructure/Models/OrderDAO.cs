using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class OrderDAO
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid OrderActivityId { get; set; }

    public Guid? AddressId { get; set; }

    public double TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public virtual OrderActivityDAO OrderActivity { get; set; } = null!;

    public virtual ICollection<OrderItemDAO> OrderItems { get; set; } = new List<OrderItemDAO>();

    public virtual ICollection<PaymentDAO> Payments { get; set; } = new List<PaymentDAO>();

    public virtual ICollection<ReviewDAO> Reviews { get; set; } = new List<ReviewDAO>();

    public virtual UserDAO User { get; set; } = null!;

    public virtual AddressDAO? Address { get; set; }
}
