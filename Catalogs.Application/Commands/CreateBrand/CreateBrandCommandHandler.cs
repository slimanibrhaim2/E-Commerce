using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Commands.CreateBrand;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Result<Guid>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IProductRepository _productRepository;
    private readonly IServiceRepository _serviceRepository;

    public CreateBrandCommandHandler(
        IBrandRepository brandRepository,
        IProductRepository productRepository,
        IServiceRepository serviceRepository)
    {
        _brandRepository = brandRepository;
        _productRepository = productRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<Result<Guid>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Brand.Name))
        {
            return Result<Guid>.Fail(
                message: "اسم العلامة التجارية مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var brand = new Brand
            {
                Name = request.Brand.Name,
                Description = request.Brand.Description,
                LogoUrl = request.Brand.LogoUrl,
                IsActive = true
            };

            // Add the brand through Brand repository
            var brandId = await _brandRepository.AddBrandAsync(brand);

            
            
            return Result<Guid>.Ok(
                data: brandId,
                message: "تم إنشاء العلامة التجارية بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"فشل في إنشاء العلامة التجارية: {ex.Message}",
                errorType: "CreateBrandFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 