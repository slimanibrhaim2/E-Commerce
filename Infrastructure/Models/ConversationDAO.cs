using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ConversationDAO
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<ConversationMemberDAO> ConversationMembers { get; set; } = new List<ConversationMemberDAO>();

    public virtual ICollection<MessageDAO> Messages { get; set; } = new List<MessageDAO>();
}
