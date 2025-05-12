using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class MediaType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductMedium> ProductMedia { get; set; } = new List<ProductMedium>();
}
