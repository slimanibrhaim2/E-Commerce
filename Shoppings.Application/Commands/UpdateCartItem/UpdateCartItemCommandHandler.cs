using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Shoppings.Application.Commands.UpdateCartItem
{
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result<bool>>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCartItemCommandHandler(ICartItemRepository cartItemRepository, IUnitOfWork unitOfWork)
        {
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(request.Id);
            if (cartItem == null)
                return Result<bool>.Fail("CartItem not found.", "NotFound", ResultStatus.NotFound);
            cartItem.Quantity = request.Quantity;
            _cartItemRepository.Update(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "updated", ResultStatus.Success);
        }
    }
} 