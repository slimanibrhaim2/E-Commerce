using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Favorite
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BaseItemId { get; set; }

    public virtual BaseItem BaseItem { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
