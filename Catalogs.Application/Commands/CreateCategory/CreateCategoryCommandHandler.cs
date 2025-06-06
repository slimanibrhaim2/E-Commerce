using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync();

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