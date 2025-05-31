using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Shoppings.Application.Commands.CreateCartItem
{
    public class CreateCartItemCommandHandler : IRequestHandler<CreateCartItemCommand, Result<Guid>>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCartItemCommandHandler(ICartItemRepository cartItemRepository, IUnitOfWork unitOfWork)
        {
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCartItemCommand request, CancellationToken cancellationToken)
        {
            if (request.CartId == Guid.Empty || request.BaseItemId == Guid.Empty)
                return Result<Guid>.Fail("CartId and BaseItemId are required.", "ValidationError", ResultStatus.ValidationError);
            if (request.Quantity <= 0)
                return Result<Guid>.Fail("Quantity must be greater than zero.", "ValidationError", ResultStatus.ValidationError);
            var cartItem = new CartItem
            {
                CartId = request.CartId,
                BaseItemId = request.BaseItemId,
                Quantity = request.Quantity
            };
            await _cartItemRepository.AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(cartItem.Id, "added", ResultStatus.Success);
        }
    }
} 