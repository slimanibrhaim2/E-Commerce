using MediatR;
using Core.Result;
using System;

namespace Shared.Contracts.Queries
{
    public record GetBaseItemIdByProductIdQuery(Guid ProductId) : IRequest<Result<Guid>>;
} 