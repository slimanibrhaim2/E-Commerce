using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;
using Shared.Contracts.Queries;

namespace Shoppings.Application.Commands.CreateCartItem
{
    public class CreateCartItemCommandHandler : IRequestHandler<CreateCartItemCommand, Result<Guid>>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public CreateCartItemCommandHandler(
            ICartItemRepository cartItemRepository, 
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<Result<Guid>> Handle(CreateCartItemCommand request, CancellationToken cancellationToken)
        {
            if (request.CartId == Guid.Empty || request.ItemId == Guid.Empty)
                return Result<Guid>.Fail("CartId and ItemId are required.", "ValidationError", ResultStatus.ValidationError);
            if (request.Quantity <= 0)
                return Result<Guid>.Fail("Quantity must be greater than zero.", "ValidationError", ResultStatus.ValidationError);

            // Try to get BaseItemId from ProductId first
            var productQuery = new GetBaseItemIdByProductIdQuery(request.ItemId);
            var productResult = await _mediator.Send(productQuery, cancellationToken);
            
            Guid baseItemId;
            if (productResult.Success)
            {
                baseItemId = productResult.Data;
            }
            else
            {
                // If not a product, try to get BaseItemId from ServiceId
                var serviceQuery = new GetBaseItemIdByServiceIdQuery(request.ItemId);
                var serviceResult = await _mediator.Send(serviceQuery, cancellationToken);
                
                if (serviceResult.Success)
                {
                    baseItemId = serviceResult.Data;
                }
                else
                {
                    return Result<Guid>.Fail(
                        "Item not found. The provided ItemId is neither a valid ProductId nor ServiceId.", 
                        "NotFound", 
                        ResultStatus.NotFound);
                }
            }

            var cartItem = new CartItem
            {
                CartId = request.CartId,
                BaseItemId = baseItemId,
                Quantity = request.Quantity
            };

            await _cartItemRepository.AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(cartItem.Id, "Cart item added successfully", ResultStatus.Success);
        }
    }
} 