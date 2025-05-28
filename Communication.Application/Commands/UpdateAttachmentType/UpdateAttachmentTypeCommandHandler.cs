using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateAttachmentType;

public class UpdateAttachmentTypeCommandHandler : IRequestHandler<UpdateAttachmentTypeCommand, Result<bool>>
{
    private readonly IAttachmentTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAttachmentTypeCommandHandler(IAttachmentTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateAttachmentTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "Attachment type not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            if (string.IsNullOrWhiteSpace(request.AttachmentType.Name))
            {
                return Result<bool>.Fail(
                    message: "Name is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            entity.Name = request.AttachmentType.Name;
            entity.UpdatedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Attachment type updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to update attachment type: {ex.Message}",
                errorType: "UpdateAttachmentTypeFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 