// Users.Application/DTOs/FollowerDTO.cs
using System;

namespace Users.Application.DTOs
{
    public class FollowerDTO
    {
        public Guid FollowerId { get; set; }    // The user who follows
        public string FollowerName { get; set; } = string.Empty;    // Name of the follower
        public string FollowerProfileUrl { get; set; } = string.Empty;    // Profile URL of the follower
        public DateTime CreatedAt { get; set; }
        public bool IsFollowed { get; set; }    // Whether the current user follows this follower back
    }

    public class FollowingDTO
    {
        public Guid FollowingId { get; set; }    // The user being followed
        public string FollowingName { get; set; } = string.Empty;    // Name of the user being followed
        public string FollowingProfileUrl { get; set; } = string.Empty;    // Profile URL of the user being followed
        public DateTime CreatedAt { get; set; }
        public bool IsFollowed { get; set; }    // Whether the current user follows this user (should always be true for following list)
    }
}
