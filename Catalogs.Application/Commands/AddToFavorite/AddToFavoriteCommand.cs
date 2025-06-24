using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.AddToFavorite;

public record AddToFavoriteCommand(AddToFavoriteDTO Favorite, Guid UserId) : IRequest<Result<Guid>>; 