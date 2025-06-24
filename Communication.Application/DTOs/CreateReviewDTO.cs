using System;

namespace Communication.Application.DTOs
{
    public class CreateReviewDTO
    {
        public Guid ItemId { get; set; }
        public Guid OrderId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
} 