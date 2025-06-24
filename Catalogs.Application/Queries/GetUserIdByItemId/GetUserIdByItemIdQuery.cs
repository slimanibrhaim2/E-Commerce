using MediatR;
using Core.Result;

namespace Catalogs.Application.Queries.GetUserIdByItemId;

public record GetUserIdByItemIdQuery(Guid ItemId) : IRequest<Result<Guid>>; 