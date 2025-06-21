using MediatR;
using Core.Result;
using System;

namespace Shared.Contracts.Queries
{
    public record GetBaseItemIdByServiceIdQuery(Guid ServiceId) : IRequest<Result<Guid>>;
} 