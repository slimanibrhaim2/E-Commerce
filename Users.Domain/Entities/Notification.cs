namespace Users.Domain.Entities;

public class Notification 
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string NotificationContenet { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

}