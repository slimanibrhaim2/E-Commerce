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
    private readonly IBaseContentRepository _baseContentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommentAggregateCommandHandler(
        ICommentRepository commentRepository,
        IBaseContentRepository baseContentRepository,
        IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _baseContentRepository = baseContentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateCommentAggregateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransaction();
        try
        {
            var dto = request.DTO;
            var userId = request.UserId;
            
            // 1. Get existing comment
            var comment = await _commentRepository.GetByIdAsync(request.Id);
            if (comment == null)
            {
                return Result<Guid>.Fail(
                    message: $"Comment with ID {request.Id} not found.",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // 2. Validate content
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return Result<Guid>.Fail(
                    message: "محتوى التعليق مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // 3. Update BaseContent
            var baseContent = await _baseContentRepository.GetByIdAsync(comment.BaseContentId);
            if (baseContent != null)
            {
                baseContent.Title = dto.Content;
                baseContent.Description = dto.Content;
                baseContent.UpdatedAt = DateTime.UtcNow;
                _baseContentRepository.Update(baseContent);
            }

            // 4. Update comment properties
            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            // 5. Update comment in repository
            _commentRepository.Update(comment);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();

            return Result<Guid>.Ok(
                data: comment.Id,
                message: "Comment updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<Guid>.Fail(
                message: $"Failed to update comment: {ex.Message}",
                errorType: "UpdateCommentAggregateFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 