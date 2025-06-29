using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Communication.Application.DTOs
{
    public class CreateMessageRequest
    {
        public Guid ConversationId { get; set; }
        public string Content { get; set; } = null!;
        public Guid ReceiverId { get; set; }
        public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
    }
} 