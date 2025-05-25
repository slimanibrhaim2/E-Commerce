using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Domain.Entities
{
    public class Favorite 
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BaseItemId { get; set; }
        public BaseItem BaseItem { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
