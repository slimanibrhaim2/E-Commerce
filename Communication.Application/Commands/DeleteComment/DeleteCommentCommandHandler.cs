using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteComment;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
{
    private readonly ICommentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentCommandHandler(ICommentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _repository.GetByIdAsync(request.Id);
            if (comment == null)
            {
                return Result<bool>.Fail(
                    message: "التعليق غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            _repository.Remove(comment);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "تم حذف التعليق بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في حذف التعليق: {ex.Message}",
                errorType: "DeleteCommentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 