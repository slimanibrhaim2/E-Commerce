using System;
using System.Collections.Generic;

namespace Communication.Application.DTOs
{
    public class AddCommentAggregateDTO
    {
        public string Content { get; set; } = null!;
        public Guid UserId { get; set; }
        public Guid BaseItemId { get; set; }
        public List<AttachmentDTO> Attachments { get; set; } = new List<AttachmentDTO>();
    }
} 