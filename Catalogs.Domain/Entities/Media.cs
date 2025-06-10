using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Domain.Entities
{
    public class Media 
    {
        public Guid Id { get; set; }

        public Media()
        {
            
        }

        public string MediaUrl { get; set; } = null!;
        public Guid MediaTypeId { get; set; }
        public MediaType MediaType { get; set; } = null!;
        public Guid BaseItemId { get; set; }
        public BaseItem BaseItem { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
