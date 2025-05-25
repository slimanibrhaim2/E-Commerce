using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Category.Name))
        {
            return Result<Guid>.Fail(
                message: "اسم التصنيف مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var category = new Category
            {
                Name = request.Category.Name,
                Description = request.Category.Description,
                ParentCategoryId = request.Category.ParentId,
                IsActive = true
            };

            var categoryId = await _categoryRepository.AddCategoryAsync(category);

            return Result<Guid>.Ok(
                data: categoryId,
                message: "تم إنشاء الفئة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"فشل في إنشاء الفئة: {ex.Message}",
                errorType: "CreateCategoryFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 