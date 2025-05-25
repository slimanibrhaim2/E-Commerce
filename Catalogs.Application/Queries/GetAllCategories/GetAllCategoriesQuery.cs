using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllCategories;

public record GetAllCategoriesQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<CategoryDTO>>>; 