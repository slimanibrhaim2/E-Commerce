using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetMyFavorites;

public record GetMyFavoritesQuery(Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<FavoriteDTO>>>; 