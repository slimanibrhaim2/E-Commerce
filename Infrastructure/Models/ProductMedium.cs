using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ProductMedium
{
    public Guid Id { get; set; }

    public Guid BaseItemId { get; set; }

    public Guid MediaTypeId { get; set; }

    public string Url { get; set; } = null!;

    public DateTime AddedAt { get; set; }

    public virtual BaseItem BaseItem { get; set; } = null!;

    public virtual MediaType MediaType { get; set; } = null!;
}
