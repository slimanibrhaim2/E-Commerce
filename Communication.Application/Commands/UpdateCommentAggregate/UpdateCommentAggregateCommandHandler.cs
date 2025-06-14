using MediatR;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateCommentAggregate;

public class UpdateCommentAggregateCommandHandler : IRequestHandler<UpdateCommentAggregateCommand, Result<Guid>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommentAggregateCommandHandler(
        ICommentRepository commentRepository,
        IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateCommentAggregateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get existing comment
            var comment = await _commentRepository.GetByIdAsync(request.Id);
            if (comment == null)
            {
                return Result<Guid>.Fail(
                    message: $"Comment with ID {request.Id} not found.",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // 2. Update comment properties
            comment.Content = request.DTO.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            // 3. Update comment in repository
            _commentRepository.Remove(comment);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(
                data: comment.Id,
                message: "Comment updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to update comment: {ex.Message}",
                errorType: "UpdateCommentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 