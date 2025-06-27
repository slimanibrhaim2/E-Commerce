using MediatR;
using Core.Result;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Interfaces;
using Shared.Contracts.Queries;
using System.Collections.Generic;
using System.Linq;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Queries.GetMyCart
{
    public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, Result<CartDTO>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMyCartQueryHandler> _logger;
        private readonly IMediator _mediator;

        public GetMyCartQueryHandler(
            ICartRepository cartRepository,
            IUnitOfWork unitOfWork,
            ILogger<GetMyCartQueryHandler> logger,
            IMediator mediator)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Result<CartDTO>> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                {
                    return Result<CartDTO>.Fail(
                        message: "معرف المستخدم مطلوب للوصول إلى سلة التسوق",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Getting or creating cart for user {UserId}", request.UserId);

                var cart = await _cartRepository.GetOrCreateCartByUserIdAsync(request.UserId);
                await _unitOfWork.SaveChangesAsync();

                var cartItemsWithDetails = new List<CartItemWithDetailsDTO>();
                
                foreach (var cartItem in cart.CartItems ?? Enumerable.Empty<CartItem>())
                {
                    // Get item details using the unified query
                    var itemDetailsQuery = new GetItemDetailsByBaseItemIdQuery(cartItem.BaseItemId);
                    var itemDetailsResult = await _mediator.Send(itemDetailsQuery, cancellationToken);

                    // Determine the original ItemId (Product ID or Service ID) from the item details
                    Guid itemId = cartItem.BaseItemId; // Default to BaseItemId if we can't resolve
                    if (itemDetailsResult.Success && itemDetailsResult.Data != null)
                    {
                        itemId = itemDetailsResult.Data.Id; // This will be the Product ID or Service ID
                    }

                    var cartItemDto = new CartItemDTO
                    {
                        Id = cartItem.Id,
                        CartId = cartItem.CartId,
                        BaseItemId = cartItem.BaseItemId,
                        ItemId = itemId,
                        Quantity = cartItem.Quantity,
                        CreatedAt = cartItem.CreatedAt,
                        UpdatedAt = cartItem.UpdatedAt,
                        DeletedAt = cartItem.DeletedAt
                    };

                    var composedDto = new CartItemWithDetailsDTO
                    {
                        CartItem = cartItemDto,
                        ItemDetails = itemDetailsResult.Success ? itemDetailsResult.Data : null
                    };

                    cartItemsWithDetails.Add(composedDto);
                }

                var cartDto = new CartDTO
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    CreatedAt = cart.CreatedAt,
                    UpdatedAt = cart.UpdatedAt,
                    DeletedAt = cart.DeletedAt,
                    CartItemsWithDetails = cartItemsWithDetails
                };

                return Result<CartDTO>.Ok(
                    data: cartDto,
                    message: "تم جلب سلة التسوق الخاصة بك بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cart for user {UserId}", request.UserId);
                return Result<CartDTO>.Fail(
                    message: $"فشل في الوصول إلى سلة التسوق: {ex.Message}",
                    errorType: "GetCartFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 