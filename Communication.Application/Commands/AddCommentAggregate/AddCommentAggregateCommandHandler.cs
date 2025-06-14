using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Communication.Application.Commands.AddCommentAggregate
{
    public class AddCommentAggregateCommandHandler : IRequestHandler<AddCommentAggregateCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepo;
        private readonly IBaseContentRepository _baseContentRepo;
        private readonly IAttachmentRepository _attachmentRepo;

        public AddCommentAggregateCommandHandler(
            IUnitOfWork unitOfWork,
            ICommentRepository commentRepo,
            IBaseContentRepository baseContentRepo,
            IAttachmentRepository attachmentRepo)
        {
            _unitOfWork = unitOfWork;
            _commentRepo = commentRepo;
            _baseContentRepo = baseContentRepo;
            _attachmentRepo = attachmentRepo;
        }

        public async Task<Result<Guid>> Handle(AddCommentAggregateCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                // 1. Validate comment
                var dto = request.DTO;
                if (string.IsNullOrWhiteSpace(dto.Content))
                    return Result<Guid>.Fail("محتوى التعليق مطلوب", "ValidationError", ResultStatus.ValidationError);
                if (dto.UserId == Guid.Empty)
                    return Result<Guid>.Fail("معرف المستخدم مطلوب", "ValidationError", ResultStatus.ValidationError);
                if (dto.BaseItemId == Guid.Empty)
                    return Result<Guid>.Fail("معرف العنصر الأساسي مطلوب", "ValidationError", ResultStatus.ValidationError);

                // 2. Create BaseContent
                var baseContent = new BaseContent
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    Title = dto.Content, // Using content as title since it's required
                    Description = dto.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _baseContentRepo.AddAsync(baseContent);
                await _unitOfWork.SaveChangesAsync();

                // 3. Create Comment
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = dto.Content,
                    UserId = dto.UserId,
                    BaseItemId = dto.BaseItemId,
                    BaseContentId = baseContent.Id,
                    CreatedAt = DateTime.UtcNow
                };
                await _commentRepo.AddAsync(comment);
                await _unitOfWork.SaveChangesAsync();

                // 4. Add Attachments
                foreach (var attachmentDto in dto.Attachments ?? Enumerable.Empty<AttachmentDTO>())
                {
                    var attachment = new Attachment
                    {
                        Id = Guid.NewGuid(),
                        BaseContentId = baseContent.Id,
                        AttachmentUrl = attachmentDto.AttachmentUrl,
                        AttachmentTypeId = attachmentDto.AttachmentTypeId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _attachmentRepo.AddAsync(attachment);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
                return Result<Guid>.Ok(comment.Id, "تم إضافة التعليق مع المرفقات بنجاح", ResultStatus.Success);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return Result<Guid>.Fail($"فشل في إضافة التعليق: {ex.Message}", "AddCommentAggregateFailed", ResultStatus.Failed, ex);
            }
        }
    }
} 