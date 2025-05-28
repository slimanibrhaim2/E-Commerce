using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid BaseContentId { get; set; }

        public Guid BaseItemId { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
        public virtual BaseContent BaseContent { get; set; } = null!;

    }
}
