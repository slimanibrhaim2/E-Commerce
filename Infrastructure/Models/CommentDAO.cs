using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class CommentDAO
{
    public Guid Id { get; set; }

    public Guid BaseContentId { get; set; }

    public Guid BaseItemId { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public virtual BaseContentDAO BaseContent { get; set; } = null!;

    public virtual BaseItemDAO BaseItem { get; set; } = null!;
}
