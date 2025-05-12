using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string NotificationContenet { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
