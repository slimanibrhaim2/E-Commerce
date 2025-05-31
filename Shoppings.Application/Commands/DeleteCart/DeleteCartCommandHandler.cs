using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;
using Shoppings.Application.Commands.DeleteCart;
using Core.Interfaces;

namespace Shoppings.Application.Commands.Handlers
{
    public class DeleteCartCommandHandler : IRequestHandler<DeleteCartCommand, Result<bool>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCartCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.Id);
            if (cart == null)
                return Result<bool>.Fail("Cart not found.", "NotFound", ResultStatus.NotFound);
            _cartRepository.Remove(cart);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "deleted", ResultStatus.Success);
        }
    }
} 