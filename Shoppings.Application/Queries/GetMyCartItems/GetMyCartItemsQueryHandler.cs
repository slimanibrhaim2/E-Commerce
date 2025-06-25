using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Shoppings.Application.Queries.GetMyCartItems
{
    public class GetMyCartItemsQueryHandler : IRequestHandler<GetMyCartItemsQuery, Result<PaginatedResult<CartItemWithDetailsDTO>>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<GetMyCartItemsQueryHandler> _logger;
        private readonly IMediator _mediator;

        public GetMyCartItemsQueryHandler(
            ICartRepository cartRepository,
            ILogger<GetMyCartItemsQueryHandler> logger,
            IMediator mediator)
        {
            _cartRepository = cartRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Result<PaginatedResult<CartItemWithDetailsDTO>>> Handle(GetMyCartItemsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                {
                    return Result<PaginatedResult<CartItemWithDetailsDTO>>.Fail(
                        message: "معرف المستخدم مطلوب لعرض محتويات سلة التسوق",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.Parameters.PageNumber < 1)
                {
                    return Result<PaginatedResult<CartItemWithDetailsDTO>>.Fail(
                        message: "رقم الصفحة يجب أن يكون 1 أو أكثر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.Parameters.PageSize < 1)
                {
                    return Result<PaginatedResult<CartItemWithDetailsDTO>>.Fail(
                        message: "حجم الصفحة يجب أن يكون 1 أو أكثر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Getting cart items for user {UserId}", request.UserId);

                // Get user's active cart
                var cart = await _cartRepository.GetActiveCartByUserIdAsync(request.UserId);
                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    var emptyResult = PaginatedResult<CartItemWithDetailsDTO>.Create(
                        new List<CartItemWithDetailsDTO>(), 
                        request.Parameters.PageNumber, 
                        request.Parameters.PageSize, 
                        0);
                    return Result<PaginatedResult<CartItemWithDetailsDTO>>.Ok(emptyResult, "سلة التسوق فارغة", ResultStatus.Success);
                }

                // Apply pagination to cart items
                var totalCount = cart.CartItems.Count;
                var paginatedItems = cart.CartItems
                    .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                    .Take(request.Parameters.PageSize)
                    .ToList();

                // Create composed DTOs with item details
                var cartItemsWithDetails = new List<CartItemWithDetailsDTO>();
                
                foreach (var cartItem in paginatedItems)
                {
                    // Get item details using the new unified query
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
                        ItemId = itemId, // Set the original Product ID or Service ID
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

                var paginated = PaginatedResult<CartItemWithDetailsDTO>.Create(cartItemsWithDetails, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
                return Result<PaginatedResult<CartItemWithDetailsDTO>>.Ok(paginated, "تم جلب محتويات سلة التسوق بنجاح", ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items for user {UserId}", request.UserId);
                return Result<PaginatedResult<CartItemWithDetailsDTO>>.Fail(
                    message: "فشل في جلب محتويات سلة التسوق",
                    errorType: "GetCartItemsFailed",
                    resultStatus: ResultStatus.Failed);
            }
        }
    }
} 