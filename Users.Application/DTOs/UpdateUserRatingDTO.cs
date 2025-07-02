using System;
using System.ComponentModel.DataAnnotations;

namespace Users.Application.DTOs
{
    public class UpdateUserRatingDTO
    {
        [Required]
        public Guid userId { set; get; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        public Guid ReviewId { get; set; }
    }
} 