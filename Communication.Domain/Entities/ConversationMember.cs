using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Domain.Entities
{
    public class ConversationMember
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual Conversation Conversation { get; set; } = null!;

    }
}
