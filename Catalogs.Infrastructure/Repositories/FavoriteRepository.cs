using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogs.Infrastructure.Repositories;

public class FavoriteRepository : BaseRepository<Favorite, FavoriteDAO>, IFavoriteRepository
{
    private readonly ECommerceContext _context;
    private readonly IMapper<FavoriteDAO, Favorite> _mapper;

    public FavoriteRepository(ECommerceContext context, IMapper<FavoriteDAO, Favorite> mapper)
        : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(Guid userId)
    {
        var favoriteDaos = await _context.Favorites
            .Include(f => f.BaseItem)
            .Where(f => f.UserId == userId && f.DeletedAt == null)
            .ToListAsync();

        return favoriteDaos.Select(f => _mapper.Map(f));
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, Guid baseItemId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.BaseItemId == baseItemId && f.DeletedAt == null);
    }
}