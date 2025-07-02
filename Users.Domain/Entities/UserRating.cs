using System;

namespace Users.Domain.Entities
{
    public class UserRating
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int NumOfReviews { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
} 