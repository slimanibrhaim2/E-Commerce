using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Brand
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public virtual ICollection<Brand> InverseParent { get; set; } = new List<Brand>();

    public virtual Brand? Parent { get; set; }

    public virtual Product Product { get; set; } = null!;
}
