using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Conversation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
