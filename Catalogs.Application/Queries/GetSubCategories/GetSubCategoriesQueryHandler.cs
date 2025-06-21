using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetSubCategories;

public class GetSubCategoriesQueryHandler : IRequestHandler<GetSubCategoriesQuery, Result<PaginatedResult<CategoryDTO>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetSubCategoriesQueryHandler> _logger;

    public GetSubCategoriesQueryHandler(ICategoryRepository categoryRepository, ILogger<GetSubCategoriesQueryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<CategoryDTO>>> Handle(GetSubCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate parent category exists
            var parentCategory = await _categoryRepository.GetCategoryByIdAsync(request.ParentId);
            if (parentCategory == null)
            {
                return Result<PaginatedResult<CategoryDTO>>.Fail(
                    message: "الفئة الأب غير موجودة",
                    errorType: "ParentCategoryNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var subcategories = await _categoryRepository.GetSubCategoriesAsync(request.ParentId);
            var subcategoryList = subcategories.ToList();

            if (!subcategoryList.Any())
            {
                return Result<PaginatedResult<CategoryDTO>>.Ok(
                    data: PaginatedResult<CategoryDTO>.Create(
                        data: new List<CategoryDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: "لا توجد فئات فرعية لهذه الفئة",
                    resultStatus: ResultStatus.Success);
            }

            var categoryDtos = subcategoryList.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentId = c.ParentCategoryId,
                IsActive = c.IsActive,
                ImageUrl = c.ImageUrl,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();

            var paginatedResult = PaginatedResult<CategoryDTO>.Create(
                data: categoryDtos,
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: categoryDtos.Count);

            return Result<PaginatedResult<CategoryDTO>>.Ok(
                data: paginatedResult,
                message: $"تم جلب {categoryDtos.Count} فئة فرعية بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الفئات الفرعية للفئة {ParentId}", request.ParentId);
            return Result<PaginatedResult<CategoryDTO>>.Fail(
                message: $"فشل في جلب الفئات الفرعية: {ex.Message}",
                errorType: "GetSubCategoriesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 