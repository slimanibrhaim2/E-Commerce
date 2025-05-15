using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ProductFeatureDAO
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ProductDAO Product { get; set; } = null!;
}
