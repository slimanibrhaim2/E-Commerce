using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Follower
{
    public Guid FollowerId { get; set; }

    public Guid FollowingId { get; set; }

    public DateTime FollowedAt { get; set; }

    public virtual User FollowerNavigation { get; set; } = null!;

    public virtual User Following { get; set; } = null!;
}
