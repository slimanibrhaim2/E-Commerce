using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;

namespace Shoppings.Application.Queries.GetAllCart
{
    public class GetAllCartQueryHandler : IRequestHandler<GetAllCartQuery, Result<PaginatedResult<Cart>>>
    {
        private readonly ICartRepository _cartRepository;
        public GetAllCartQueryHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Result<PaginatedResult<Cart>>> Handle(GetAllCartQuery request, CancellationToken cancellationToken)
        {
            var all = await _cartRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<Cart>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<Cart>>.Ok(paginated, "تم جلب سلات التسوق بنجاح", ResultStatus.Success);
        }
    }
} 