using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Queries.GetBrandById;

public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandDTO>>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandByIdQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Result<BrandDTO>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var brand = await _brandRepository.GetBrandByIdAsync(request.Id);
            if (brand == null)
            {
                return Result<BrandDTO>.Fail(
                    message: "لم يتم العثور على العلامة التجارية",
                    errorType: "BrandNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var brandDto = new BrandDTO
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                LogoUrl = brand.LogoUrl,
                IsActive = brand.IsActive
            };

            return Result<BrandDTO>.Ok(
                data: brandDto,
                message: "تم العثور على العلامة التجارية بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<BrandDTO>.Fail(
                message: $"فشل في الحصول على العلامة التجارية: {ex.Message}",
                errorType: "GetBrandFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 