using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;

namespace Shoppings.Application.Commands.UpdateMyCartItem
{
    public record UpdateMyCartItemCommand(
        Guid UserId, 
        Guid ItemId, 
        int Quantity,
        PaginationParameters Parameters) : IRequest<Result>;
} 