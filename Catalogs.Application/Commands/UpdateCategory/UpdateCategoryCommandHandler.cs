using Catalogs.Domain.Repositories;
using Core.Result;
using MediatR;
using Catalogs.Domain.Entities;
using Catalogs.Application.DTOs;
using Core.Interfaces;

namespace Catalogs.Application.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: result,
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