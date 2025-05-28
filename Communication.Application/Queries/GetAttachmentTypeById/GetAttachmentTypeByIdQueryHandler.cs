using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;

namespace Communication.Application.Queries.GetAttachmentTypeById;

public class GetAttachmentTypeByIdQueryHandler : IRequestHandler<GetAttachmentTypeByIdQuery, Result<AttachmentTypeDTO>>
{
    private readonly IAttachmentTypeRepository _repository;

    public GetAttachmentTypeByIdQueryHandler(IAttachmentTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AttachmentTypeDTO>> Handle(GetAttachmentTypeByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<AttachmentTypeDTO>.Fail(
                    message: "Attachment type not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            var dto = new AttachmentTypeDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt
            };
            return Result<AttachmentTypeDTO>.Ok(
                data: dto,
                message: "Attachment type found successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<AttachmentTypeDTO>.Fail(
                message: $"Failed to get attachment type: {ex.Message}",
                errorType: "GetAttachmentTypeFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 