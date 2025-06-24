using System;
using System.Threading.Tasks;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Result;
using MediatR;

namespace Communication.Application.Queries.GetCommentById
{
    public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, Result<CommentDTO>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IBaseContentRepository _baseContentRepository;

        public GetCommentByIdQueryHandler(ICommentRepository commentRepository, IBaseContentRepository baseContentRepository)
        {
            _commentRepository = commentRepository;
            _baseContentRepository = baseContentRepository;
        }

        public async Task<Result<CommentDTO>> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _commentRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<CommentDTO>.Fail(
                    message: "التعليق غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Fetch BaseContent for the comment
            var baseContent = await _baseContentRepository.GetByIdAsync(entity.BaseContentId);
            
            var dto = new CommentDTO
            {
                Id = entity.Id,
                UserId = baseContent?.UserId ?? Guid.Empty,
                Title = baseContent?.Title ?? string.Empty,
                Description = baseContent?.Description ?? string.Empty,
                BaseContentId = entity.BaseContentId,
                BaseItemId = entity.BaseItemId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt,
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
            return Result<CommentDTO>.Ok(
                data: dto,
                message: "تم جلب التعليق بنجاح",
                resultStatus: ResultStatus.Success);
        }
    }
} 