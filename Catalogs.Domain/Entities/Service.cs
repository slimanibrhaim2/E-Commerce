using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Domain.Entities
{
    public class Service : BaseItem
    {
        public Guid Id { get; set; }
        public Guid BaseItemId { get; set; }
        public string ServiceType { get; set; } = null!;
        public int Duration { get; set; }
        public new bool IsAvailable { get; set; } = true;
        public List<ServiceFeature>? Features { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
