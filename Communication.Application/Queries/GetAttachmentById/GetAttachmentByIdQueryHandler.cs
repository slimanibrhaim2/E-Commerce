using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;

namespace Communication.Application.Queries.GetAttachmentById;

public class GetAttachmentByIdQueryHandler : IRequestHandler<GetAttachmentByIdQuery, Result<AttachmentDTO>>
{
    private readonly IAttachmentRepository _repository;

    public GetAttachmentByIdQueryHandler(IAttachmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AttachmentDTO>> Handle(GetAttachmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<AttachmentDTO>.Fail(
                    message: "Attachment not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            var dto = new AttachmentDTO
            {
                Id = entity.Id,
                BaseContentId = entity.BaseContentId,
                AttachmentUrl = entity.AttachmentUrl,
                AttachmentTypeId = entity.AttachmentTypeId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt
            };
            return Result<AttachmentDTO>.Ok(
                data: dto,
                message: "Attachment retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<AttachmentDTO>.Fail(
                message: $"Failed to get attachment: {ex.Message}",
                errorType: "GetAttachmentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 