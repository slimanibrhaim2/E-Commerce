using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
