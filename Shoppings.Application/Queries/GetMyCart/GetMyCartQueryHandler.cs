using MediatR;
using Core.Result;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Interfaces;

namespace Shoppings.Application.Queries.GetMyCart
{
    public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, Result<CartDTO>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMyCartQueryHandler> _logger;

        public GetMyCartQueryHandler(
            ICartRepository cartRepository,
            IUnitOfWork unitOfWork,
            ILogger<GetMyCartQueryHandler> logger)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CartDTO>> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId == Guid.Empty)
                {
                    return Result<CartDTO>.Fail(
                        message: "معرف المستخدم مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Getting or creating cart for user {UserId}", request.UserId);

                var cart = await _cartRepository.GetOrCreateCartByUserIdAsync(request.UserId);
                await _unitOfWork.SaveChangesAsync();

                var cartDto = new CartDTO
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    CreatedAt = cart.CreatedAt,
                    UpdatedAt = cart.UpdatedAt,
                    DeletedAt = cart.DeletedAt,
                    CartItems = cart.CartItems?.Select(ci => new CartItemDTO
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        BaseItemId = ci.BaseItemId,
                        Quantity = ci.Quantity,
                        CreatedAt = ci.CreatedAt,
                        UpdatedAt = ci.UpdatedAt,
                        DeletedAt = ci.DeletedAt
                    }).ToList() ?? new List<CartItemDTO>()
                };

                return Result<CartDTO>.Ok(
                    data: cartDto,
                    message: "تم جلب سلة التسوق بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cart for user {UserId}", request.UserId);
                return Result<CartDTO>.Fail(
                    message: $"فشل في جلب سلة التسوق: {ex.Message}",
                    errorType: "GetMyCartFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 