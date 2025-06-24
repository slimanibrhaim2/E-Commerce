using MediatR;
using Core.Result;
using Shoppings.Application.DTOs;
 
namespace Shoppings.Application.Queries.GetMyCart
{
    public record GetMyCartQuery(Guid UserId) : IRequest<Result<CartDTO>>;
} 