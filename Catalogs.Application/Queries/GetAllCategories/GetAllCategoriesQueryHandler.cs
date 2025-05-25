using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<PaginatedResult<CategoryDTO>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<PaginatedResult<CategoryDTO>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var categoryDtos = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentId = c.ParentCategoryId,
                IsActive = c.IsActive,
                ImageUrl = c.ImageUrl
            }).ToList();

            var paginatedResult = PaginatedResult<CategoryDTO>.Create(
                data: categoryDtos,
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: categoryDtos.Count);

            return Result<PaginatedResult<CategoryDTO>>.Ok(
                data: paginatedResult,
                message: "تم جلب الفئات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<CategoryDTO>>.Fail(
                message: $"فشل في جلب الفئات: {ex.Message}",
                errorType: "GetCategoriesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 