using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Application.DTOs
{
    public class CreatCategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }
}
