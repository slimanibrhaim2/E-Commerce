using Catalogs.Domain.Repositories;
using Core.Result;
using MediatR;
using Catalogs.Domain.Entities;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Category.Name))
        {
            return Result<bool>.Fail(
                message: "اسم التصنيف مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            // Get the category using Category repository
            var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);
            if (category == null)
            {
                return Result<bool>.Fail(
                    message: "لم يتم العثور على التصنيف",
                    errorType: "CategoryNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Update category properties
            category.Name = request.Category.Name;
            category.Description = request.Category.Description;
            category.ImageUrl = request.Category.ImageUrl;
            category.IsActive = request.Category.IsActive;
            category.ParentCategoryId = request.Category.ParentId;

            // Update the category using Category repository
            var result = await _categoryRepository.UpdateCategoryAsync(request.Id, category);

            if (!result)
            {
                return Result<bool>.Fail(
                    message: "فشل في تحديث التصنيف",
                    errorType: "UpdateCategoryFailed",
                    resultStatus: ResultStatus.Failed);
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث التصنيف بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في تحديث التصنيف: {ex.Message}",
                errorType: "UpdateCategoryFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 