using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Service
{
    public Guid Id { get; set; }

    public Guid BaseItemId { get; set; }

    public DateTime Duration { get; set; }

    public string Location { get; set; } = null!;

    public virtual BaseItem BaseItem { get; set; } = null!;
}
