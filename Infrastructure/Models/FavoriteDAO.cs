using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class FavoriteDAO
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BaseItemId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual UserDAO User { get; set; } = null!;

    public virtual BaseItemDAO BaseItem { get; set; } = null!;
}
