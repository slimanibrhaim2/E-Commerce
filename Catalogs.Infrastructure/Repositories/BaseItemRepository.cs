using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;

namespace Catalogs.Infrastructure.Repositories;

public class BaseItemRepository : BaseRepository<BaseItem, BaseItemDAO>, IBaseItemRepository
{
    public BaseItemRepository(ECommerceContext context, IMapper<BaseItemDAO, BaseItem> mapper)
        : base(context, mapper)
    {
    }
} 