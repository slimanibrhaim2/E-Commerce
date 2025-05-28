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
    public class MessageRepository : BaseRepository<Message, MessageDAO>, IMessageRepository
    {
        public MessageRepository(ECommerceContext ctx, IMapper<MessageDAO, Message> mapper)
            : base(ctx, mapper) { }
    }
}
