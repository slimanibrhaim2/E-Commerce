using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;
using Shared.Contracts.Queries;
using Microsoft.Extensions.Logging;

namespace Shoppings.Application.Commands.RemoveFromMyCart
{
    public class RemoveFromMyCartCommandHandler : IRequestHandler<RemoveFromMyCartCommand, Result>
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

        public async Task<Result> Handle(RemoveFromMyCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty || request.ItemId == Guid.Empty)
                {
                    return Result.Fail(
                        message: "معرف المستخدم ومعرف المنتج/الخدمة مطلوبان لحذف العنصر من سلة التسوق",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Removing item {ItemId} from user {UserId} cart", request.ItemId, request.UserId);

                // Get user's cart
                var cart = await _cartRepository.GetActiveCartByUserIdAsync(request.UserId);
                if (cart == null)
                {
                    return Result.Fail(
                        message: "سلة التسوق الخاصة بك غير موجودة أو فارغة",
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
                        return Result.Fail(
                            message: "المنتج أو الخدمة غير موجودة. تأكد من صحة معرف المنتج أو الخدمة",
                            errorType: "NotFound",
                            resultStatus: ResultStatus.NotFound);
                    }
                }

                // Find the cart item
                var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.BaseItemId == baseItemId && ci.DeletedAt == null);
                if (cartItem == null)
                {
                    return Result.Fail(
                        message: "المنتج أو الخدمة غير موجودة في سلة التسوق الخاصة بك",
                        errorType: "CartItemNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Remove the cart item (soft delete)
                cartItem.DeletedAt = DateTime.UtcNow;
                cartItem.UpdatedAt = DateTime.UtcNow;
                
                // Save changes
                var updateResult = await _cartItemRepository.UpdateAsync(cartItem);
                if (!updateResult)
                {
                    return Result.Fail(
                        message: "فشل في حذف المنتج/الخدمة من سلة التسوق",
                        errorType: "DeleteFailed",
                        resultStatus: ResultStatus.Failed);
                }
                
                await _unitOfWork.SaveChangesAsync();
                
                return Result.Ok(
                    message: "تم حذف المنتج/الخدمة من سلة التسوق بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove item {ItemId} from user {UserId} cart", request.ItemId, request.UserId);
                return Result.Fail(
                    message: $"فشل في حذف المنتج/الخدمة من سلة التسوق: {ex.Message}",
                    errorType: "RemoveFromCartFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 