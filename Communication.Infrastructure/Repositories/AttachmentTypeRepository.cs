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
    public class AttachmentTypeRepository : BaseRepository<AttachmentType, AttachmentTypeDAO>, IAttachmentTypeRepository
    {
        public AttachmentTypeRepository(ECommerceContext ctx, IMapper<AttachmentTypeDAO, AttachmentType> mapper)
            : base(ctx, mapper) { }
    }
}
