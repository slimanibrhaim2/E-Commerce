using System;
using Core.Result;
using MediatR;
using Shared.Contracts.DTOs;

namespace Shared.Contracts.Queries
{
    public record GetProviderIdByOrderIdQuery(Guid OrderId) : IRequest<Result<OrderProviderDTO>>;

} 