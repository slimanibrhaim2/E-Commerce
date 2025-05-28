using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateAttachment;

public class UpdateAttachmentCommandHandler : IRequestHandler<UpdateAttachmentCommand, Result<bool>>
{
    private readonly IAttachmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAttachmentCommandHandler(IAttachmentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "Attachment not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            if (request.Attachment.BaseContentId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "BaseContentId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (string.IsNullOrWhiteSpace(request.Attachment.AttachmentUrl))
            {
                return Result<bool>.Fail(
                    message: "AttachmentUrl is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Attachment.AttachmentTypeId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "AttachmentTypeId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            entity.BaseContentId = request.Attachment.BaseContentId;
            entity.AttachmentUrl = request.Attachment.AttachmentUrl;
            entity.AttachmentTypeId = request.Attachment.AttachmentTypeId;
            entity.UpdatedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Attachment updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to update attachment: {ex.Message}",
                errorType: "UpdateAttachmentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 