using MediatR;
using Core.Result;

namespace Catalogs.Application.Queries.CheckIsFavorite;
 
public record CheckIsFavoriteQuery(Guid ItemId, Guid UserId) : IRequest<Result<bool>>;