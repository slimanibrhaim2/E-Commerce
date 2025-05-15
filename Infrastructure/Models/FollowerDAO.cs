using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class FollowerDAO
{
    public Guid Id { get; set; }

    public Guid FollowerId { get; set; }

    public Guid FollowingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual UserDAO Follower { get; set; } = null!;

    public virtual UserDAO Following { get; set; } = null!;
}
