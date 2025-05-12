using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class CartItem
{
    public Guid Id { get; set; }

    public Guid CartId { get; set; }

    public Guid BaseItemId { get; set; }

    public DateTime CreationDate { get; set; }

    public double Quantity { get; set; }

    public virtual BaseItem BaseItem { get; set; } = null!;

    public virtual Cart Cart { get; set; } = null!;
}
