using MediatR;
using Communication.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteCommentAggregate;

public class DeleteCommentAggregateCommandHandler : IRequestHandler<DeleteCommentAggregateCommand, Result<bool>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentAggregateCommandHandler(
        ICommentRepository commentRepository,
        IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteCommentAggregateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get existing comment
            var comment = await _commentRepository.GetByIdAsync(request.Id);
            if (comment == null)
            {
                return Result<bool>.Fail(
                    message: $"Comment with ID {request.Id} not found.",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // 2. Soft delete comment
            comment.DeletedAt = DateTime.UtcNow;
            _commentRepository.Remove(comment);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: true,
                message: "Comment deleted successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to delete comment: {ex.Message}",
                errorType: "DeleteCommentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 