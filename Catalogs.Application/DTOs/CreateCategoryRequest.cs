using System.ComponentModel.DataAnnotations;

namespace Catalogs.Application.DTOs
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        public Guid? ParentId { get; set; }

        public bool IsActive { get; set; } = true;
    }
} 