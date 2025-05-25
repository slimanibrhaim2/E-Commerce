using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDTO>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryDTO>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);
            if (category == null)
            {
                return Result<CategoryDTO>.Fail(
                    message: "الفئة غير موجودة",
                    errorType: "CategoryNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var categoryDto = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentId = category.ParentCategoryId,
                IsActive = category.IsActive,
                ImageUrl = category.ImageUrl
            };

            return Result<CategoryDTO>.Ok(
                data: categoryDto,
                message: "تم جلب الفئة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<CategoryDTO>.Fail(
                message: $"فشل في جلب الفئة: {ex.Message}",
                errorType: "GetCategoryFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 