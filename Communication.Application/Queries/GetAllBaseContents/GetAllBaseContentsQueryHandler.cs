using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllBaseContents;

public class GetAllBaseContentsQueryHandler : IRequestHandler<GetAllBaseContentsQuery, Result<PaginatedResult<BaseContentDTO>>>
{
    private readonly IBaseContentRepository _repository;

    public GetAllBaseContentsQueryHandler(IBaseContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResult<BaseContentDTO>>> Handle(GetAllBaseContentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var totalCount = entities.Count();
            var data = entities
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .Select(e => new BaseContentDTO
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    Title = e.Title,
                    Description = e.Description,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    DeletedAt = e.DeletedAt
                })
                .ToList();
            var paginated = Core.Pagination.PaginatedResult<BaseContentDTO>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<BaseContentDTO>>.Ok(
                paginated,
                message: "Base contents retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<BaseContentDTO>>.Fail(
                message: $"Failed to get base contents: {ex.Message}",
                errorType: "GetAllBaseContentsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 