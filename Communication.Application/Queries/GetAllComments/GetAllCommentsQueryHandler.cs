using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Application.Queries.GetAllComments;

public class GetAllCommentsQueryHandler : IRequestHandler<GetAllCommentsQuery, Result<List<CommentDTO>>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IBaseContentRepository _baseContentRepository;

    public GetAllCommentsQueryHandler(ICommentRepository commentRepository, IBaseContentRepository baseContentRepository)
    {
        _commentRepository = commentRepository;
        _baseContentRepository = baseContentRepository;
    }

    public async Task<Result<List<CommentDTO>>> Handle(GetAllCommentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comments = await _commentRepository.GetAllAsync();
            var dtos = comments.Select(c => new CommentDTO
            {
                Id = c.Id,
                BaseContentId = c.BaseContentId,
                BaseItemId = c.BaseItemId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                DeletedAt = c.DeletedAt,
                // BaseContent = c.BaseContent != null ? new BaseContentDTO { ... } : null
            }).ToList();
            return Result<List<CommentDTO>>.Ok(
                data: dtos,
                message: "Comments retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<CommentDTO>>.Fail(
                message: $"Failed to get comments: {ex.Message}",
                errorType: "GetAllCommentsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 