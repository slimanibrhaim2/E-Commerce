using MediatR;
using Core.Result;
using Shared.Contracts.DTOs;

namespace Catalogs.Application.Queries.GetItemIdByFavoriteId;

public record GetItemIdByFavoriteIdQuery(Guid FavoriteId) : IRequest<Result<ItemIdResponseDTO>>; 