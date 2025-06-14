using System;
using System.Collections.Generic;
using Communication.Domain.Entities;

namespace Communication.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public Guid UserId { get; set; }
        public Guid BaseItemId { get; set; }
        public Guid BaseContentId { get; set; }
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual BaseContent BaseContent { get; set; } = null!;
    }
}
