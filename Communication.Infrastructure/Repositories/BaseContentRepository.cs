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
    public class BaseContentRepository : BaseRepository<BaseContent, BaseContentDAO>, IBaseContentRepository
    {
        public BaseContentRepository(ECommerceContext ctx, IMapper<BaseContentDAO, BaseContent> mapper)
            : base(ctx, mapper) { }
    }
}
