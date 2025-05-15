using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class CartDAO
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual UserDAO User { get; set; } = null!;

    public virtual ICollection<CartItemDAO> CartItems { get; set; } = new List<CartItemDAO>();
}
