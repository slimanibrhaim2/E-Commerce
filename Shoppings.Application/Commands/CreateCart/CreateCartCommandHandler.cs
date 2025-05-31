using Core.Interfaces;
using Core.Result;
using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;

namespace Shoppings.Application.Commands.CreateCart
{
    public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, Result<Guid>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCartCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
                return Result<Guid>.Fail("UserId is required.", "ValidationError", ResultStatus.ValidationError);
            var cart = new Cart { UserId = request.UserId };
            await _cartRepository.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(cart.Id,"cart created", ResultStatus.Success);
        }
    }
} 