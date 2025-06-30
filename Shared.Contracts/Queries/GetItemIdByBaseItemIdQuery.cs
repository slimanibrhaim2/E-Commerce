using MediatR;
using Core.Result;
using System;
using Shared.Contracts.DTOs;

namespace Shared.Contracts.Queries
{
    public record GetItemIdByBaseItemIdQuery(Guid BaseItemId) : IRequest<Result<ItemIdResponseDTO>>;
} 