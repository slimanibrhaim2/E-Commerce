using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Application.DTOs
{
    public class CreateBrandDTO
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string LogoUrl { get; set; }

        public bool IsActive { get; set; }

    }
}
