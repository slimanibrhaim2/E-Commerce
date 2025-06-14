using Communication.Domain.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Domain.Repositories
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId);
        Task<Conversation?> GetConversationByMembersAsync(Guid userId1, Guid userId2);
    }
}
