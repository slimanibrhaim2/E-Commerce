using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;
using Shared.Contracts.Queries;
using Microsoft.Extensions.Logging;

namespace Shoppings.Application.Commands.UpdateMyCartItem
{
    public class UpdateMyCartItemCommandHandler : IRequestHandler<UpdateMyCartItemCommand, Result<bool>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateMyCartItemCommandHandler> _logger;

        public UpdateMyCartItemCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<UpdateMyCartItemCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(UpdateMyCartItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty || request.ItemId == Guid.Empty)
                {
                    return Result<bool>.Fail(
                        message: "معرف المستخدم ومعرف المنتج/الخدمة مطلوبان لتحديث كمية العنصر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.Quantity <= 0)
                {
                    return Result<bool>.Fail(
                        message: "يجب أن تكون الكمية الجديدة أكبر من صفر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Updating item {ItemId} quantity to {Quantity} in user {UserId} cart", 
                    request.ItemId, request.Quantity, request.UserId);

                // Get user's cart
                var cart = await _cartRepository.GetActiveCartByUserIdAsync(request.UserId);
                if (cart == null)
                {
                    return Result<bool>.Fail(
                        message: "سلة التسوق الخاصة بك غير موجودة",
                        errorType: "CartNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Resolve ItemId to BaseItemId
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
                        return Result<bool>.Fail(
                            message: "المنتج أو الخدمة غير موجودة. تأكد من صحة معرف المنتج أو الخدمة",
                            errorType: "NotFound",
                            resultStatus: ResultStatus.NotFound);
                    }
                }

                // Find the cart item
                var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.BaseItemId == baseItemId && ci.DeletedAt == null);
                if (cartItem == null)
                {
                    return Result<bool>.Fail(
                        message: "المنتج أو الخدمة غير موجودة في سلة التسوق الخاصة بك",
                        errorType: "CartItemNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Update quantity
                cartItem.Quantity = request.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
                
                await _unitOfWork.SaveChangesAsync();
                
                return Result<bool>.Ok(
                    data: true,
                    message: "تم تحديث كمية المنتج/الخدمة في سلة التسوق بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update item {ItemId} quantity in user {UserId} cart", request.ItemId, request.UserId);
                return Result<bool>.Fail(
                    message: $"فشل في تحديث كمية المنتج/الخدمة في سلة التسوق: {ex.Message}",
                    errorType: "UpdateCartItemFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 