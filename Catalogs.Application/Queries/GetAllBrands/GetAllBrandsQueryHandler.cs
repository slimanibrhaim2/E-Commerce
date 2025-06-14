using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Catalogs.Domain.Entities;
using System.Linq;
using Core.Pagination;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetAllBrands;

public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, Result<PaginatedResult<BrandDTO>>>
{
    private readonly IBrandRepository _repository;
    private readonly ILogger<GetAllBrandsQueryHandler> _logger;

    public GetAllBrandsQueryHandler(
        IBrandRepository repository,
        ILogger<GetAllBrandsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<BrandDTO>>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<BrandDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<BrandDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var brands = (await _repository.GetAllAsync()).ToList();
            
            if (!brands.Any())
            {
                return Result<PaginatedResult<BrandDTO>>.Ok(
                    data: PaginatedResult<BrandDTO>.Create(
                        data: new List<BrandDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: "No brands found",
                    resultStatus: ResultStatus.Success);
            }

            var totalCount = brands.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = brands.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                LogoUrl = b.LogoUrl,
                IsActive = b.IsActive,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();

            var paginated = PaginatedResult<BrandDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<BrandDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} brands out of {totalCount} total brands",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving brands");
            return Result<PaginatedResult<BrandDTO>>.Fail(
                message: "Failed to retrieve brands due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving brands: {Message}", ex.Message);
            return Result<PaginatedResult<BrandDTO>>.Fail(
                message: "An unexpected error occurred while retrieving brands. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
}

// Helper class to compare brands by ID
public class BrandComparer : IEqualityComparer<Brand>
{
    public bool Equals(Brand x, Brand y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Id.Equals(y.Id);
    }

    public int GetHashCode(Brand obj)
    {
        return obj.Id.GetHashCode();
    }
} 