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
            var data = comments
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    BaseContentId = c.BaseContentId,
                    BaseItemId = c.BaseItemId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    DeletedAt = c.DeletedAt,
                })
                .ToList();
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