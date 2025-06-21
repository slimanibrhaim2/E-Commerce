using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetSubCategories;

public record GetSubCategoriesQuery(Guid ParentId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<CategoryDTO>>>; 