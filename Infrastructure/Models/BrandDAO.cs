using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class BrandDAO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ProductDAO> Products { get; set; } = new List<ProductDAO>();
}
