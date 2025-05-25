using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;

namespace Catalogs.Infrastructure.Repositories;

public class FavoriteRepository : BaseRepository<Favorite, FavoriteDAO>, IFavoriteRepository
{
    public FavoriteRepository(ECommerceContext context, IMapper<FavoriteDAO, Favorite> mapper)
        : base(context, mapper)
    {
    }
} 