using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllBrands;

public record GetAllBrandsQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<BrandDTO>>>; 