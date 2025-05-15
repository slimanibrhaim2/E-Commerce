using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class CategoryDAO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid? ParentId { get; set; }

    public virtual ICollection<BaseItemDAO> BaseItems { get; set; } = new List<BaseItemDAO>();

    public virtual ICollection<CategoryDAO> InverseParent { get; set; } = new List<CategoryDAO>();

    public virtual CategoryDAO? Parent { get; set; }
}
