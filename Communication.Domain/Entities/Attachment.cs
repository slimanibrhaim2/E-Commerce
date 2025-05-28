using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Domain.Entities
{
    public class Attachment
    {

        public Guid Id { get; set; }

        public Guid BaseContentId { get; set; }

        public string AttachmentUrl { get; set; } = null!;

        public Guid AttachmentTypeId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual BaseContent BaseContent { get; set; } = null!;
    }
}
