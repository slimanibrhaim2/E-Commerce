using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class MediaDAO
{
    public Guid Id { get; set; }

    public Guid BaseItemId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public Guid MediaTypeId { get; set; }
    public virtual MediaTypeDAO MediaType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual BaseItemDAO BaseItem { get; set; } = null!;
}
