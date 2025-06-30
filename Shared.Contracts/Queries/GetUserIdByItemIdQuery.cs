using MediatR;
using Core.Result;

namespace Shared.Contracts.Queries;

public record GetUserIdByItemIdQuery(
    Guid ItemId
) : IRequest<Result<Guid>>; 