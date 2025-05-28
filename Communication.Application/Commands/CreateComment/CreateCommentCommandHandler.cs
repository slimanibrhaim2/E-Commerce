using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Guid>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommentCommandHandler(ICommentRepository commentRepository, IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Comment.BaseContentId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "BaseContentId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Comment.BaseItemId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "BaseItemId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var comment = new Comment
            {
                BaseContentId = request.Comment.BaseContentId,
                BaseItemId = request.Comment.BaseItemId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(
                data: comment.Id,
                message: "Comment created successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to create comment: {ex.Message}",
                errorType: "CreateCommentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 