using System;
using System.Collections.Generic;

namespace Communication.Application.DTOs
{
    public class AddCommentAggregateDTO
    {
        public string Content { get; set; } = null!;
        public Guid ItemId { get; set; }
        public List<AttachmentDTO> Attachments { get; set; } = new List<AttachmentDTO>();
    }
} 