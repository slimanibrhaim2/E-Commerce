using Core;

namespace Users.Domain;

public class Notification: BaseEntity
{

    public Guid UserId { get; set; }

    public string NotificationContenet { get; set; } = null!;

    public bool? IsRead { get; set; }
    
    public DateTime? ReadAt { get; set; }

}