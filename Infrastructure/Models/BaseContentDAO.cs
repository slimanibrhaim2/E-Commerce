using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class BaseContentDAO
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual UserDAO User { get; set; } = null!;

    public virtual ICollection<AttachmentDAO> Attachments { get; set; } = new List<AttachmentDAO>();
}
