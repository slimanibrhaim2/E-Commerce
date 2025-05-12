using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid BaseContentId { get; set; }

    public Guid ReceiverId { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public Guid ConversationId { get; set; }

    public virtual BaseContent BaseContent { get; set; } = null!;

    public virtual Conversation Conversation { get; set; } = null!;
}
