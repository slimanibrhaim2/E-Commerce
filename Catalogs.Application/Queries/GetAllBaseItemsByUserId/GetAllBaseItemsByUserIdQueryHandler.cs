using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllBaseItemsByUserId;

public class GetAllBaseItemsByUserIdQueryHandler : IRequestHandler<GetAllBaseItemsByUserIdQuery, Result<PaginatedResult<BaseItemDTO>>>
{
    private readonly IBaseItemRepository _repository;

    public GetAllBaseItemsByUserIdQueryHandler(IBaseItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResult<BaseItemDTO>>> Handle(GetAllBaseItemsByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = (await _repository.GetAllByUserIdAsync(request.UserId)).ToList();
            var totalCount = entities.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var pagedEntities = entities.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var dtos = pagedEntities.Select(entity => new BaseItemDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                IsAvailable = entity.IsAvailable,
                CategoryId = entity.CategoryId,
                UserId = entity.UserId
            }).ToList();
            var paginated = PaginatedResult<BaseItemDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<BaseItemDTO>>.Ok(
                data: paginated,
                message: "Base items retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<BaseItemDTO>>.Fail(
                message: $"Failed to get base items: {ex.Message}",
                errorType: "GetAllBaseItemsByUserIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 