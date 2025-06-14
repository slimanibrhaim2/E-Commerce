using System;
using System.Collections.Generic;

namespace Communication.Application.DTOs
{
    public class AddMessageAggregateDTO
    {
        public Guid ConversationId { get; set; }
        public string Content { get; set; } = null!;
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public List<AttachmentDTO> Attachments { get; set; } = new List<AttachmentDTO>();
    }
} 