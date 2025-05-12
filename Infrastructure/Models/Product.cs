using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public Guid BaseItemId { get; set; }

    public double Quantity { get; set; }

    public virtual BaseItem BaseItem { get; set; } = null!;

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

    public virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new List<ProductFeature>();
}
