using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;
using Shared.Contracts.Queries;
using Microsoft.Extensions.Logging;

namespace Shoppings.Application.Commands.AddToMyCart
{
    public class AddToMyCartCommandHandler : IRequestHandler<AddToMyCartCommand, Result<Guid>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<AddToMyCartCommandHandler> _logger;

        public AddToMyCartCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<AddToMyCartCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(AddToMyCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty || request.ItemId == Guid.Empty)
                {
                    return Result<Guid>.Fail(
                        message: "معرف المستخدم ومعرف المنتج/الخدمة مطلوبان لإضافة العنصر إلى سلة التسوق",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.Quantity <= 0)
                {
                    return Result<Guid>.Fail(
                        message: "يجب أن تكون كمية المنتج/الخدمة أكبر من صفر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Adding item {ItemId} to user {UserId} cart", request.ItemId, request.UserId);

                // Resolve ItemId to BaseItemId first
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
                            message: "المنتج أو الخدمة غير موجودة. تأكد من صحة معرف المنتج أو الخدمة",
                            errorType: "NotFound",
                            resultStatus: ResultStatus.NotFound);
                    }
                }

                // Get or create user's cart (this returns domain entity, but we need to work at DAO level)
                var cart = await _cartRepository.GetOrCreateCartByUserIdAsync(request.UserId);

                // Check if item already exists in cart by querying the repository directly
                var existingCartItem = await _cartItemRepository.GetByCartIdAndBaseItemIdAsync(cart.Id, baseItemId);
                if (existingCartItem != null)
                {
                    // Update quantity using repository method that handles tracking properly
                    existingCartItem.Quantity += request.Quantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;
                    
                    await _cartItemRepository.UpdateAsync(existingCartItem);
                    await _unitOfWork.SaveChangesAsync();
                    
                    return Result<Guid>.Ok(
                        data: existingCartItem.Id,
                        message: "تم تحديث كمية المنتج/الخدمة في سلة التسوق بنجاح",
                        resultStatus: ResultStatus.Success);
                }
                else
                {
                    // Add new cart item
                    var cartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        BaseItemId = baseItemId,
                        Quantity = request.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _cartItemRepository.AddAsync(cartItem);
                    await _unitOfWork.SaveChangesAsync();
                    
                    return Result<Guid>.Ok(
                        data: cartItem.Id,
                        message: "تم إضافة المنتج/الخدمة إلى سلة التسوق بنجاح",
                        resultStatus: ResultStatus.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add item {ItemId} to user {UserId} cart", request.ItemId, request.UserId);
                return Result<Guid>.Fail(
                    message: $"فشل في إضافة المنتج/الخدمة إلى سلة التسوق: {ex.Message}",
                    errorType: "AddToCartFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 