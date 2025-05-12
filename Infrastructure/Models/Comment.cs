using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Comment
{
    public Guid Id { get; set; }

    public Guid BaseContentId { get; set; }

    public Guid BaseItemId { get; set; }

    public virtual BaseContent BaseContent { get; set; } = null!;

    public virtual BaseItem BaseItem { get; set; } = null!;
}
