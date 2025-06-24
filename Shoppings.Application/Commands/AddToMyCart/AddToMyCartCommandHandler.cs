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
                        message: "معرف المستخدم ومعرف العنصر مطلوبان",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.Quantity <= 0)
                {
                    return Result<Guid>.Fail(
                        message: "يجب أن تكون الكمية أكبر من صفر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Adding item {ItemId} to user {UserId} cart", request.ItemId, request.UserId);

                // Get or create user's cart
                var cart = await _cartRepository.GetOrCreateCartByUserIdAsync(request.UserId);

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
                        return Result<Guid>.Fail(
                            message: "العنصر غير موجود. المعرف المدخل ليس معرف منتج أو خدمة صالح",
                            errorType: "NotFound",
                            resultStatus: ResultStatus.NotFound);
                    }
                }

                // Check if item already exists in cart
                var existingCartItem = cart.CartItems?.FirstOrDefault(ci => ci.BaseItemId == baseItemId);
                if (existingCartItem != null)
                {
                    // Update quantity
                    existingCartItem.Quantity += request.Quantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;
                    _cartItemRepository.Update(existingCartItem);
                    
                    await _unitOfWork.SaveChangesAsync();
                    return Result<Guid>.Ok(
                        data: existingCartItem.Id,
                        message: "تم تحديث كمية العنصر في سلة التسوق بنجاح",
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
                        message: "تم إضافة العنصر إلى سلة التسوق بنجاح",
                        resultStatus: ResultStatus.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add item {ItemId} to user {UserId} cart", request.ItemId, request.UserId);
                return Result<Guid>.Fail(
                    message: $"فشل في إضافة العنصر إلى سلة التسوق: {ex.Message}",
                    errorType: "AddToMyCartFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 