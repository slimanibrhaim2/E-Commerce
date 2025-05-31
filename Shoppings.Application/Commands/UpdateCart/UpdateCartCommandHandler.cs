using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Shoppings.Application.Commands.UpdateCart
{
    public class UpdateCartCommandHandler : IRequestHandler<UpdateCartCommand, Result<bool>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCartCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.Id);
            if (cart == null)
                return Result<bool>.Fail("Cart not found.", "NotFound", ResultStatus.NotFound);
            cart.UserId = request.UserId;
            _cartRepository.Update(cart);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "updated", ResultStatus.Success);
        }
    }
} 