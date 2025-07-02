using System;

namespace Infrastructure.Models
{
    public class UserRatingDAO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int NumOfReviews { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation property
        public virtual UserDAO User { get; set; }
    }
} 