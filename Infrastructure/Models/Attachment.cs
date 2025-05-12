using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Attachment
{
    public Guid Id { get; set; }

    public Guid BaseContentId { get; set; }

    public string ContentUrl { get; set; } = null!;

    public Guid AttachmentTypeId { get; set; }

    public virtual AttachmentType AttachmentType { get; set; } = null!;

    public virtual BaseContent BaseContent { get; set; } = null!;
}
