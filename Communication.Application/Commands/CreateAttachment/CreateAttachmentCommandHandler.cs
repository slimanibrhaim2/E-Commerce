using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.CreateAttachment;

public class CreateAttachmentCommandHandler : IRequestHandler<CreateAttachmentCommand, Result<Guid>>
{
    private readonly IAttachmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAttachmentCommandHandler(IAttachmentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Attachment.BaseContentId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "BaseContentId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (string.IsNullOrWhiteSpace(request.Attachment.AttachmentUrl))
            {
                return Result<Guid>.Fail(
                    message: "AttachmentUrl is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Attachment.AttachmentTypeId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "AttachmentTypeId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var entity = new Attachment
            {
                BaseContentId = request.Attachment.BaseContentId,
                AttachmentUrl = request.Attachment.AttachmentUrl,
                AttachmentTypeId = request.Attachment.AttachmentTypeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(
                data: entity.Id,
                message: "Attachment created successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to create attachment: {ex.Message}",
                errorType: "CreateAttachmentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 