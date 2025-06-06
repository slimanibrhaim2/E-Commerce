using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(IProductRepository repo, ILogger<GetAllProductsQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _repo.GetAllAsync();
            var productDtos = products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                SKU = p.SKU,
                StockQuantity = p.StockQuantity,
                IsAvailable = p.IsAvailable,
                // Add other properties as needed
            }).ToList();
            var totalCount = productDtos.Count;

            var paginatedProducts = PaginatedResult<ProductDTO>.Create(
                data: productDtos,
                pageNumber: request.Pagination.PageNumber,
                pageSize: request.Pagination.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginatedProducts,
                message: "تم جلب المنتجات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all products");
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "حدث خطأ أثناء جلب المنتجات",
                errorType: "GetAllProductsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 