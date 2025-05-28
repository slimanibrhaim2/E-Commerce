using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;

namespace Communication.Application.Queries.GetBaseContentById;

public class GetBaseContentByIdQueryHandler : IRequestHandler<GetBaseContentByIdQuery, Result<BaseContentDTO>>
{
    private readonly IBaseContentRepository _repository;

    public GetBaseContentByIdQueryHandler(IBaseContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BaseContentDTO>> Handle(GetBaseContentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<BaseContentDTO>.Fail(
                    message: "Base content not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            var dto = new BaseContentDTO
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Title = entity.Title,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt
            };
            return Result<BaseContentDTO>.Ok(
                data: dto,
                message: "Base content retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<BaseContentDTO>.Fail(
                message: $"Failed to get base content: {ex.Message}",
                errorType: "GetBaseContentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 