using MediatR;
using Shared.Contracts.DTOs;
using Core.Result;
using System;

namespace Shared.Contracts.Queries
{
    public record GetItemDetailsByBaseItemIdQuery(Guid BaseItemId) : IRequest<Result<ItemDetailsDTO>>;
} 