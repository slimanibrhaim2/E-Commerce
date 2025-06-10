using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Domain.Entities
{
    public class ProductFeature 
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid BaseItemId { get; set; }
        public BaseItem BaseItem { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
