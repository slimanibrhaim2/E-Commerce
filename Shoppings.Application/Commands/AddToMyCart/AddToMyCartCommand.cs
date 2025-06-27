using MediatR;
using Core.Result;
 
namespace Shoppings.Application.Commands.AddToMyCart
{
    public record AddToMyCartCommand(Guid UserId, Guid ItemId, int Quantity) : IRequest<Result>;
} 