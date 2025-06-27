using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;
using Shared.Contracts.Queries;
using Microsoft.Extensions.Logging;
using Core.Pagination;
using Shoppings.Application.DTOs;
using System.Collections.Generic;
using System.Linq;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Commands.UpdateMyCartItem
{
    public class UpdateMyCartItemCommandHandler : IRequestHandler<UpdateMyCartItemCommand, Result>
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

        public async Task<Result> Handle(UpdateMyCartItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty || request.ItemId == Guid.Empty)
                {
                    return Result.Fail(
                        message: "معرف المستخدم ومعرف المنتج/الخدمة مطلوبان لتحديث كمية العنصر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.Quantity < 0)
                {
                    return Result.Fail(
                        message: "يجب أن تكون الكمية الجديدة 0 أو أكثر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Updating item {ItemId} quantity to {Quantity} in user {UserId} cart", 
                    request.ItemId, request.Quantity, request.UserId);

                // Get user's cart
                var cart = await _cartRepository.GetActiveCartByUserIdAsync(request.UserId);
                if (cart == null)
                {
                    return Result.Fail(
                        message: "سلة التسوق فارغة",
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

                // Find the cart item using the repository to ensure we get fresh data
                var cartItem = await _cartItemRepository.GetByCartIdAndBaseItemIdAsync(cart.Id, baseItemId);
                if (cartItem == null)
                {
                    return Result.Fail(
                        message: "المنتج أو الخدمة غير موجودة في سلة التسوق الخاصة بك",
                        errorType: "CartItemNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // If quantity is 0, soft delete the item
                if (request.Quantity == 0)
                {
                    cartItem.DeletedAt = DateTime.UtcNow;
                    cartItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Update the cart item
                    cartItem.Quantity = request.Quantity;
                    cartItem.UpdatedAt = DateTime.UtcNow;
                }

                // Save changes
                var updateResult = await _cartItemRepository.UpdateAsync(cartItem);
                if (!updateResult)
                {
                    return Result.Fail(
                        message: request.Quantity == 0 
                            ? "فشل في حذف المنتج/الخدمة من سلة التسوق"
                            : "فشل في تحديث كمية المنتج/الخدمة في سلة التسوق",
                        errorType: "UpdateFailed",
                        resultStatus: ResultStatus.Failed);
                }

                await _unitOfWork.SaveChangesAsync();
                
                return Result.Ok(
                    message: "تم تحديث كمية المنتج/الخدمة في سلة التسوق بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update item {ItemId} quantity to {Quantity} in user {UserId} cart", 
                    request.ItemId, request.Quantity, request.UserId);
                return Result.Fail(
                    message: $"فشل في تحديث كمية المنتج/الخدمة في سلة التسوق: {ex.Message}",
                    errorType: "UpdateCartItemFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 