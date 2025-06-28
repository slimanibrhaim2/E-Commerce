using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Catalogs.Application.DTOs
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        public Guid? ParentId { get; set; }

        [JsonIgnore] // This will exclude the property from request/response serialization
        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
