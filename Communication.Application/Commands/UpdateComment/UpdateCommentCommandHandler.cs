using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateComment;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<bool>>
{
    private readonly ICommentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommentCommandHandler(ICommentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "Comment not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            if (request.Comment.BaseContentId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "BaseContentId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Comment.BaseItemId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "BaseItemId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            entity.BaseContentId = request.Comment.BaseContentId;
            entity.BaseItemId = request.Comment.BaseItemId;
            entity.UpdatedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Comment updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to update comment: {ex.Message}",
                errorType: "UpdateCommentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 