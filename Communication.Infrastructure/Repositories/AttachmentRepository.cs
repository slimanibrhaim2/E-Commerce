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
    public class AttachmentRepository : BaseRepository<Attachment, AttachmentDAO>, IAttachmentRepository
    {
        public AttachmentRepository(ECommerceContext ctx, IMapper<AttachmentDAO, Attachment> mapper)
            : base(ctx, mapper) { }
    }
}
