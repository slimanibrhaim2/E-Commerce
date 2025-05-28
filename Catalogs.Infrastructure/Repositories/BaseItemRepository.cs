using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalogs.Infrastructure.Repositories;

public class BaseItemRepository : BaseRepository<BaseItem, BaseItemDAO>, IBaseItemRepository
{
    public BaseItemRepository(ECommerceContext context, IMapper<BaseItemDAO, BaseItem> mapper)
        : base(context, mapper)
    {
    }

    public async Task<IEnumerable<BaseItem>> GetAllByUserIdAsync(Guid userId)
    {
        var daos = await _dbSet.Where(b => b.UserId == userId).ToListAsync();
        return daos.Select(d => _mapper.Map(d));
    }
} 