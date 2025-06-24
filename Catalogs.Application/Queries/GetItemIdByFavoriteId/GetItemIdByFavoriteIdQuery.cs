using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetItemIdByFavoriteId;

public record GetItemIdByFavoriteIdQuery(Guid FavoriteId) : IRequest<Result<ItemIdResponseDTO>>; 