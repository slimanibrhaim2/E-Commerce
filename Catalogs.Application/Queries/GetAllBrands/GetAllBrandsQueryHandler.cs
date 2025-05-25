using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Catalogs.Domain.Entities;
using System.Linq;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllBrands;

public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, Result<PaginatedResult<BrandDTO>>>
{
    private readonly IBrandRepository _brandRepository;

    public GetAllBrandsQueryHandler(
        IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Result<PaginatedResult<BrandDTO>>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all brands from the brand repository
            var allBrands = await _brandRepository.GetAllBrandsAsync();
            var brandDtos = allBrands.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                LogoUrl = b.LogoUrl,
                IsActive = b.IsActive
            }).ToList();

            var paginatedResult = PaginatedResult<BrandDTO>.Create(
                data: brandDtos,
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: brandDtos.Count);

            return Result<PaginatedResult<BrandDTO>>.Ok(
                data: paginatedResult,
                message: "تم جلب العلامات التجارية بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<BrandDTO>>.Fail(
                message: $"فشل في جلب العلامات التجارية: {ex.Message}",
                errorType: "InternalServerError",
                resultStatus: ResultStatus.InternalServerError,
                exception: ex);
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