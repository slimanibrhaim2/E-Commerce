using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Commands.DeleteBrand;

public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBrandRepository _brandRepository;

    public DeleteBrandCommandHandler(
        IProductRepository productRepository,
        IServiceRepository serviceRepository,
        IBrandRepository brandRepository)
    {
        _productRepository = productRepository;
        _serviceRepository = serviceRepository;
        _brandRepository = brandRepository;
    }

    public async Task<Result<bool>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (request.Id == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف العلامة التجارية مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            // Get all products and services associated with this brand
            var products = await _productRepository.GetByBrand(request.Id);
            var services = await _serviceRepository.GetByBrand(request.Id);

            // Remove brand associations from all products
            foreach (var product in products)
            {
                await _productRepository.RemoveBrandFromProductAsync(product.Id, request.Id);
            }

            // Remove brand associations from all services
            foreach (var service in services)
            {
                await _serviceRepository.RemoveBrandFromServiceAsync(service.Id, request.Id);
            }

            // Delete the brand using Brand repository
            var result = await _brandRepository.DeleteBrandAsync(request.Id);

            if (!result)
            {
                return Result<bool>.Fail(
                    message: "لم يتم العثور على العلامة التجارية",
                    errorType: "BrandNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم حذف العلامة التجارية بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في حذف العلامة التجارية: {ex.Message}",
                errorType: "DeleteBrandFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 