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

        public GetCommentByIdQueryHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
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
            var dto = new CommentDTO
            {
                Id = entity.Id,
                BaseContentId = entity.BaseContentId,
                BaseItemId = entity.BaseItemId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt,
                // BaseContent = entity.BaseContent != null ? new BaseContentDTO { ... } : null
            };
            return Result<CommentDTO>.Ok(
                data: dto,
                message: "تم جلب التعليق بنجاح",
                resultStatus: ResultStatus.Success);
        }
    }
} 