using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Communication.Application.Queries.GetAllBaseContents;

public class GetAllBaseContentsQueryHandler : IRequestHandler<GetAllBaseContentsQuery, Result<List<BaseContentDTO>>>
{
    private readonly IBaseContentRepository _repository;

    public GetAllBaseContentsQueryHandler(IBaseContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<BaseContentDTO>>> Handle(GetAllBaseContentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var dtos = entities.Select(e => new BaseContentDTO
            {
                Id = e.Id,
                UserId = e.UserId,
                Title = e.Title,
                Description = e.Description,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                DeletedAt = e.DeletedAt
            }).ToList();
            return Result<List<BaseContentDTO>>.Ok(
                data: dtos,
                message: "Base contents retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<BaseContentDTO>>.Fail(
                message: $"Failed to get base contents: {ex.Message}",
                errorType: "GetAllBaseContentsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 