using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ConversationMember
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
