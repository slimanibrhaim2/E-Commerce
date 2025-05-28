using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteAttachment;

public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result<bool>>
{
    private readonly IAttachmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAttachmentCommandHandler(IAttachmentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
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
            _repository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Attachment deleted successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to delete attachment: {ex.Message}",
                errorType: "DeleteAttachmentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 