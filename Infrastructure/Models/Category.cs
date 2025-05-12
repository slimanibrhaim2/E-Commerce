using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public virtual ICollection<BaseItem> BaseItems { get; set; } = new List<BaseItem>();

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category? Parent { get; set; }
}
