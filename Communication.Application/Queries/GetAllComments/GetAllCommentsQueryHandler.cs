using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllComments;

public class GetAllCommentsQueryHandler : IRequestHandler<GetAllCommentsQuery, Result<PaginatedResult<CommentDTO>>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IBaseContentRepository _baseContentRepository;

    public GetAllCommentsQueryHandler(ICommentRepository commentRepository, IBaseContentRepository baseContentRepository)
    {
        _commentRepository = commentRepository;
        _baseContentRepository = baseContentRepository;
    }

    public async Task<Result<PaginatedResult<CommentDTO>>> Handle(GetAllCommentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comments = await _commentRepository.GetAllAsync();
            var totalCount = comments.Count();
            
            // Get paginated comments
            var paginatedComments = comments
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .ToList();

            // Fetch BaseContent for each comment
            var data = new List<CommentDTO>();
            foreach (var comment in paginatedComments)
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
                data.Add(commentDto);
            }

            var paginated = Core.Pagination.PaginatedResult<CommentDTO>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<CommentDTO>>.Ok(
                paginated,
                message: "تم جلب التعليقات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<CommentDTO>>.Fail(
                message: $"فشل في جلب التعليقات: {ex.Message}",
                errorType: "GetAllCommentsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 