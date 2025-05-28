using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Infrastructure.Repositories
{
    public class ConversationMemberRepository : BaseRepository<ConversationMember, ConversationMemberDAO>, IConversationMemberRepository
    {
        public ConversationMemberRepository(ECommerceContext ctx, IMapper<ConversationMemberDAO, ConversationMember> mapper)
            : base(ctx, mapper) { }
    }
}
