using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteBaseContent;

public class DeleteBaseContentCommandHandler : IRequestHandler<DeleteBaseContentCommand, Result<bool>>
{
    private readonly IBaseContentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBaseContentCommandHandler(IBaseContentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteBaseContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "Base content not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            _repository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Base content deleted successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to delete base content: {ex.Message}",
                errorType: "DeleteBaseContentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 