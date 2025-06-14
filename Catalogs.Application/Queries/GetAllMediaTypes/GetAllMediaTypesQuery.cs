using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllMediaTypes;

public record GetAllMediaTypesQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<MediaTypeDTO>>>; 