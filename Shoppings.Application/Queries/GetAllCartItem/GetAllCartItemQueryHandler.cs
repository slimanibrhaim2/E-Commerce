using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;

namespace Shoppings.Application.Queries.GetAllCartItem
{
    public class GetAllCartItemQueryHandler : IRequestHandler<GetAllCartItemQuery, Result<PaginatedResult<CartItem>>>
    {
        private readonly ICartItemRepository _cartItemRepository;
        public GetAllCartItemQueryHandler(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public async Task<Result<PaginatedResult<CartItem>>> Handle(GetAllCartItemQuery request, CancellationToken cancellationToken)
        {
            var all = await _cartItemRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<CartItem>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<CartItem>>.Ok(paginated, "تم جلب عناصر سلات التسوق بنجاح", ResultStatus.Success);
        }
    }
} 