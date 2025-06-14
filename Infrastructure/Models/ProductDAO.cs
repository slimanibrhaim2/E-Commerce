using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ProductDAO
{
    public Guid Id { get; set; }

    public Guid BaseItemId { get; set; }

    public string SKU { get; set; } = null!;

    public int StockQuantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual BaseItemDAO BaseItem { get; set; } = null!;

    public virtual ICollection<BrandDAO> Brands { get; set; } = new List<BrandDAO>();

    public virtual ICollection<ProductFeatureDAO> ProductFeatures { get; set; } = new List<ProductFeatureDAO>();
}
