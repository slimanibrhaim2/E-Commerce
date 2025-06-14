using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Infrastructure.Repositories
{
    public class ConversationRepository : BaseRepository<Conversation, ConversationDAO>, IConversationRepository
    {
        public ConversationRepository(ECommerceContext ctx, IMapper<ConversationDAO, Conversation> mapper)
            : base(ctx, mapper) { }

        public async Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId)
        {
            var daos = await _dbSet
                .Include(c => c.ConversationMembers)
                .Where(c => c.ConversationMembers.Any(m => m.UserId == userId))
                .ToListAsync();
            return daos.Select(d => _mapper.Map(d));
        }

        public async Task<Conversation?> GetConversationByMembersAsync(Guid userId1, Guid userId2)
        {
            var dao = await _dbSet
                .Include(c => c.ConversationMembers)
                .Where(c => c.ConversationMembers.Count == 2 && // Only direct conversations
                           c.ConversationMembers.Any(m => m.UserId == userId1) &&
                           c.ConversationMembers.Any(m => m.UserId == userId2))
                .FirstOrDefaultAsync();

            return dao != null ? _mapper.Map(dao) : null;
        }
    }
}
