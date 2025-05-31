using Catalogs.Domain.Repositories;
using Core.Result;
using MediatR;
using Catalogs.Domain.Entities;
using Core.Interfaces;

namespace Catalogs.Application.Commands.UpdateBrand;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result<bool>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBrandCommandHandler(
        IBrandRepository brandRepository,
        IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Brand.Name))
        {
            return Result<bool>.Fail(
                message: "اسم العلامة التجارية مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            // Get the brand using Brand repository
            var brand = await _brandRepository.GetBrandByIdAsync(request.Id);
            if (brand == null)
            {
                return Result<bool>.Fail(
                    message: "لم يتم العثور على العلامة التجارية",
                    errorType: "BrandNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Update brand properties
            brand.Name = request.Brand.Name;
            brand.Description = request.Brand.Description;
            brand.LogoUrl = request.Brand.LogoUrl;

            // Update the brand using Brand repository
            var result = await _brandRepository.UpdateBrandAsync(request.Id, brand);
            await _unitOfWork.SaveChangesAsync();

            if (!result)
            {
                return Result<bool>.Fail(
                    message: "فشل في تحديث العلامة التجارية",
                    errorType: "UpdateBrandFailed",
                    resultStatus: ResultStatus.Failed);
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث العلامة التجارية بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في تحديث العلامة التجارية: {ex.Message}",
                errorType: "UpdateBrandFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 