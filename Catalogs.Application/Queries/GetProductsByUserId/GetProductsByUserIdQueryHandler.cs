using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalogs.Application.Queries.GetProductsByUserId;

public class GetProductsByUserIdQueryHandler : IRequestHandler<GetProductsByUserIdQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repository;

    public GetProductsByUserIdQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = (await _repository.GetProductsByUserIdAsync(request.UserId)).ToList();
            var totalCount = products.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var dtos = paged.Select(p => new ProductDTO
            {
                Id = p.Id,
                SKU = p.SKU,
                StockQuantity = p.StockQuantity,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                IsAvailable = p.IsAvailable,
                UserId = p.UserId
            }).ToList();
            var paginated = PaginatedResult<ProductDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginated,
                message: "Products retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: $"Failed to get products: {ex.Message}",
                errorType: "GetProductsByUserIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 