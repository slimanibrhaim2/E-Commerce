using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class CartItemDAO
{
    public Guid Id { get; set; }

    public Guid CartId { get; set; }

    public Guid BaseItemId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual BaseItemDAO BaseItem { get; set; } = null!;

    public virtual CartDAO Cart { get; set; } = null!;
}
