using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ServiceDAO
{
    public Guid Id { get; set; }

    public Guid BaseItemId { get; set; }

    public string ServiceType { get; set; } = null!;

    public int Duration { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual BaseItemDAO BaseItem { get; set; } = null!;
    public virtual ICollection<ServiceFeatureDAO> ServiceFeatures { get; set; } = new List<ServiceFeatureDAO>();
    public virtual ICollection<BrandDAO> Brands { get; set; } = new List<BrandDAO>();
}
