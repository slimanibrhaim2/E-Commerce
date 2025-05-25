using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Application.DTOs
{
    public class CreateMediaDTO
    {
        public string? Url { get; set; }
        public Guid MediaTypeId { get; set; }
    }
}
