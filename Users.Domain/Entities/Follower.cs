namespace Users.Domain.Entities;

public class Follower 
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public User FollowerUser { get; set; } = null!;
    public User Following { get; set; } = null!;

    public Follower()
    {
        CreatedAt = DateTime.Now;
    }
}