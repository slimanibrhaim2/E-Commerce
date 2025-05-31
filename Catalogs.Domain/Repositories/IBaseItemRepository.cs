using Core.Interfaces;
using Catalogs.Domain.Entities;
using Core.Pagination;

namespace Catalogs.Domain.Repositories;

public interface IBaseItemRepository : IRepository<BaseItem>
{
    Task<IEnumerable<BaseItem>> GetAllByUserIdAsync(Guid userId);
} 