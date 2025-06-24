using Core.Interfaces;
using Catalogs.Domain.Entities;

namespace Catalogs.Domain.Repositories;

public interface IFavoriteRepository : IRepository<Favorite>
{
    Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(Guid userId);
    Task<bool> IsFavoriteAsync(Guid userId, Guid baseItemId);
} 