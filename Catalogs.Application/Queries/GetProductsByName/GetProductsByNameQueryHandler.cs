using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalogs.Application.Queries.GetProductsByName;

public class GetProductsByNameQueryHandler : IRequestHandler<GetProductsByNameQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repository;

    public GetProductsByNameQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = (await _repository.GetProductsByNameAsync(request.Name)).ToList();
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
                errorType: "GetProductsByNameFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 