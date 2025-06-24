using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteFromFavorite;

public record DeleteFromFavoriteCommand(Guid ItemId, Guid UserId) : IRequest<Result<bool>>; 