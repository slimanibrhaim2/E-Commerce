using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Cart
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User User { get; set; } = null!;
}
