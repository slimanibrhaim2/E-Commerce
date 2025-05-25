// Users.Application/DTOs/FollowerDTO.cs
using System;

namespace Users.Application.DTOs
{
    public class FollowerDTO
    {
        public Guid Id { get; set; }    // Follower record Id
        public Guid FollowerId { get; set; }    // The user who follows
        public Guid FollowingId { get; set; }    // The user being followed
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
