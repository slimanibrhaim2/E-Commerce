using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Shoppings.Application.Queries.GetMyCartItems
{
    public class GetMyCartItemsQueryHandler : IRequestHandler<GetMyCartItemsQuery, Result<PaginatedResult<CartItemDTO>>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<GetMyCartItemsQueryHandler> _logger;

        public GetMyCartItemsQueryHandler(
            ICartRepository cartRepository,
            ILogger<GetMyCartItemsQueryHandler> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<CartItemDTO>>> Handle(GetMyCartItemsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                {
                    return Result<PaginatedResult<CartItemDTO>>.Fail(
                        message: "معرف المستخدم مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.Parameters.PageNumber < 1)
                {
                    return Result<PaginatedResult<CartItemDTO>>.Fail(
                        message: "رقم الصفحة يجب أن يكون أكبر من أو يساوي 1",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.Parameters.PageSize < 1)
                {
                    return Result<PaginatedResult<CartItemDTO>>.Fail(
                        message: "حجم الصفحة يجب أن يكون أكبر من أو يساوي 1",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Getting cart items for user {UserId}", request.UserId);

                // Get user's cart
                var cart = await _cartRepository.GetActiveCartByUserIdAsync(request.UserId);
                if (cart == null)
                {
                    // Return empty result if no cart exists
                    var emptyResult = PaginatedResult<CartItemDTO>.Create(
                        data: new List<CartItemDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0);

                    return Result<PaginatedResult<CartItemDTO>>.Ok(
                        data: emptyResult,
                        message: "لا توجد عناصر في سلة التسوق",
                        resultStatus: ResultStatus.Success);
                }

                // Get cart items with pagination
                var allCartItems = cart.CartItems?.Where(ci => ci.DeletedAt == null).ToList() ?? new List<Shoppings.Domain.Entities.CartItem>();
                var totalCount = allCartItems.Count;
                
                var paginatedItems = allCartItems
                    .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                    .Take(request.Parameters.PageSize)
                    .Select(ci => new CartItemDTO
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        BaseItemId = ci.BaseItemId,
                        Quantity = ci.Quantity,
                        CreatedAt = ci.CreatedAt,
                        UpdatedAt = ci.UpdatedAt,
                        DeletedAt = ci.DeletedAt
                    }).ToList();

                var paginatedResult = PaginatedResult<CartItemDTO>.Create(
                    data: paginatedItems,
                    pageNumber: request.Parameters.PageNumber,
                    pageSize: request.Parameters.PageSize,
                    totalCount: totalCount);

                return Result<PaginatedResult<CartItemDTO>>.Ok(
                    data: paginatedResult,
                    message: "تم جلب عناصر سلة التسوق بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cart items for user {UserId}", request.UserId);
                return Result<PaginatedResult<CartItemDTO>>.Fail(
                    message: $"فشل في جلب عناصر سلة التسوق: {ex.Message}",
                    errorType: "GetMyCartItemsFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 