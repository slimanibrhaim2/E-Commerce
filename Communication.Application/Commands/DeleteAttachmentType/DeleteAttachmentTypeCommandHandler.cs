using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteAttachmentType;

public class DeleteAttachmentTypeCommandHandler : IRequestHandler<DeleteAttachmentTypeCommand, Result<bool>>
{
    private readonly IAttachmentTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAttachmentTypeCommandHandler(IAttachmentTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteAttachmentTypeCommand request, CancellationToken cancellationToken)
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
            _repository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Attachment type deleted successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to delete attachment type: {ex.Message}",
                errorType: "DeleteAttachmentTypeFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 