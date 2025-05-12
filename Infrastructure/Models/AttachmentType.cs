using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class AttachmentType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
