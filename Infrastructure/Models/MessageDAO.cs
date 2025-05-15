using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class MessageDAO
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }

    public Guid BaseContentId { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual BaseContentDAO BaseContent { get; set; } = null!;

    public virtual ConversationDAO Conversation { get; set; } = null!;

    public virtual UserDAO Sender { get; set; } = null!;
}
