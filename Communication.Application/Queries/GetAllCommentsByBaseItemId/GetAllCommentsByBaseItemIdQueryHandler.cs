using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Application.Queries.GetAllCommentsByBaseItemId;

public class GetAllCommentsByBaseItemIdQueryHandler : IRequestHandler<GetAllCommentsByBaseItemIdQuery, Result<List<CommentDTO>>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IBaseContentRepository _baseContentRepository;

    public GetAllCommentsByBaseItemIdQueryHandler(ICommentRepository commentRepository, IBaseContentRepository baseContentRepository)
    {
        _commentRepository = commentRepository;
        _baseContentRepository = baseContentRepository;
    }

    public async Task<Result<List<CommentDTO>>> Handle(GetAllCommentsByBaseItemIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comments = await _commentRepository.GetAllByBaseItemIdAsync(request.BaseItemId);
            
            // Fetch BaseContent for each comment
            var dtos = new List<CommentDTO>();
            foreach (var comment in comments)
            {
                var baseContent = await _baseContentRepository.GetByIdAsync(comment.BaseContentId);
                var commentDto = new CommentDTO
                {
                    Id = comment.Id,
                    UserId = baseContent?.UserId ?? Guid.Empty,
                    Title = baseContent?.Title ?? string.Empty,
                    Description = baseContent?.Description ?? string.Empty,
                    BaseContentId = comment.BaseContentId,
                    BaseItemId = comment.BaseItemId,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt,
                    DeletedAt = comment.DeletedAt,
                    BaseContent = baseContent != null ? new BaseContentDTO
                    {
                        Id = baseContent.Id,
                        UserId = baseContent.UserId,
                        Title = baseContent.Title,
                        Description = baseContent.Description,
                        CreatedAt = baseContent.CreatedAt,
                        UpdatedAt = baseContent.UpdatedAt,
                        DeletedAt = baseContent.DeletedAt
                    } : null!
                };
                dtos.Add(commentDto);
            }
            
            return Result<List<CommentDTO>>.Ok(
                data: dtos,
                message: "Comments retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<CommentDTO>>.Fail(
                message: $"Failed to get comments: {ex.Message}",
                errorType: "GetAllCommentsByBaseItemIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}