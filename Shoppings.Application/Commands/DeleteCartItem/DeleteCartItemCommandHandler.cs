using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Shoppings.Application.Commands.Handlers
{
    public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, Result<bool>>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCartItemCommandHandler(ICartItemRepository cartItemRepository, IUnitOfWork unitOfWork)
        {
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(request.Id);
            if (cartItem == null)
                return Result<bool>.Fail("CartItem not found.", "NotFound", ResultStatus.NotFound);
            _cartItemRepository.Remove(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "deleted", ResultStatus.Success);
        }
    }
} 