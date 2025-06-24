using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;
using Shared.Contracts.Queries;
using Microsoft.Extensions.Logging;

namespace Shoppings.Application.Commands.RemoveFromMyCart
{
    public class RemoveFromMyCartCommandHandler : IRequestHandler<RemoveFromMyCartCommand, Result<bool>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<RemoveFromMyCartCommandHandler> _logger;

        public RemoveFromMyCartCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<RemoveFromMyCartCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(RemoveFromMyCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty || request.ItemId == Guid.Empty)
                {
                    return Result<bool>.Fail(
                        message: "معرف المستخدم ومعرف العنصر مطلوبان",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Removing item {ItemId} from user {UserId} cart", request.ItemId, request.UserId);

                // Get user's cart
                var cart = await _cartRepository.GetActiveCartByUserIdAsync(request.UserId);
                if (cart == null)
                {
                    return Result<bool>.Fail(
                        message: "سلة التسوق غير موجودة",
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
                            message: "العنصر غير موجود. المعرف المدخل ليس معرف منتج أو خدمة صالح",
                            errorType: "NotFound",
                            resultStatus: ResultStatus.NotFound);
                    }
                }

                // Find the cart item
                var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.BaseItemId == baseItemId && ci.DeletedAt == null);
                if (cartItem == null)
                {
                    return Result<bool>.Fail(
                        message: "العنصر غير موجود في سلة التسوق",
                        errorType: "CartItemNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Remove the cart item (soft delete)
                cartItem.DeletedAt = DateTime.UtcNow;
                _cartItemRepository.Update(cartItem);
                
                await _unitOfWork.SaveChangesAsync();
                
                return Result<bool>.Ok(
                    data: true,
                    message: "تم حذف العنصر من سلة التسوق بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove item {ItemId} from user {UserId} cart", request.ItemId, request.UserId);
                return Result<bool>.Fail(
                    message: $"فشل في حذف العنصر من سلة التسوق: {ex.Message}",
                    errorType: "RemoveFromMyCartFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 