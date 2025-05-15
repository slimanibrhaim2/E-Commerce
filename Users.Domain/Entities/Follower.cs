using Core.Abstraction;

namespace Users.Domain.Entities;

public class Follower : BaseEntity
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime FollowedAt { get; set; }

    // Navigation properties
    public User FollowerUser { get; set; } = null!;
    public User Following { get; set; } = null!;

    public Follower()
    {
        FollowedAt = DateTime.Now;
    }
}