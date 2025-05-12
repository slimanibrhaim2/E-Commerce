using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class BaseContent
{
    public Guid Id { get; set; }

    public string? ContentText { get; set; }

    public DateTime CreateAt { get; set; }

    public Guid CreateByUserId { get; set; }

    public Guid? ParentId { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User CreateByUser { get; set; } = null!;

    public virtual ICollection<BaseContent> InverseParent { get; set; } = new List<BaseContent>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual BaseContent? Parent { get; set; }
}
