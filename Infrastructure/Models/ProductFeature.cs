using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ProductFeature
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
