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

namespace Communication.Application.Commands.AddMessageAggregate
{
    public class AddMessageAggregateCommandHandler : IRequestHandler<AddMessageAggregateCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageRepository _messageRepo;
        private readonly IBaseContentRepository _baseContentRepo;
        private readonly IAttachmentRepository _attachmentRepo;
        private readonly IConversationRepository _conversationRepo;

        public AddMessageAggregateCommandHandler(
            IUnitOfWork unitOfWork,
            IMessageRepository messageRepo,
            IBaseContentRepository baseContentRepo,
            IAttachmentRepository attachmentRepo,
            IConversationRepository conversationRepo)
        {
            _unitOfWork = unitOfWork;
            _messageRepo = messageRepo;
            _baseContentRepo = baseContentRepo;
            _attachmentRepo = attachmentRepo;
            _conversationRepo = conversationRepo;
        }

        public async Task<Result<Guid>> Handle(AddMessageAggregateCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                // 1. Validate message
                var dto = request.DTO;
                if (string.IsNullOrWhiteSpace(dto.Content))
                    return Result<Guid>.Fail("محتوى الرسالة مطلوب", "ValidationError", ResultStatus.ValidationError);
                if (dto.SenderId == Guid.Empty)
                    return Result<Guid>.Fail("معرف المرسل مطلوب", "ValidationError", ResultStatus.ValidationError);
                if (dto.ReceiverId == Guid.Empty)
                    return Result<Guid>.Fail("معرف المستلم مطلوب", "ValidationError", ResultStatus.ValidationError);

                // 2. Get or create conversation
                var conversation = await _conversationRepo.GetConversationByMembersAsync(dto.SenderId, dto.ReceiverId);
                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        Id = Guid.NewGuid(),
                        Title = dto.Content,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _conversationRepo.AddAsync(conversation);
                    await _unitOfWork.SaveChangesAsync();
                }

                // 3. Create BaseContent
                var baseContent = new BaseContent
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.SenderId,
                    Title = dto.Content,
                    Description = dto.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _baseContentRepo.AddAsync(baseContent);
                await _unitOfWork.SaveChangesAsync();

                // 4. Create Message
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversation.Id,
                    SenderId = dto.SenderId,
                    BaseContentId = baseContent.Id,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _messageRepo.AddAsync(message);
                await _unitOfWork.SaveChangesAsync();

                // 5. Add Attachments
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
                return Result<Guid>.Ok(message.Id, "تم إضافة الرسالة مع المرفقات بنجاح", ResultStatus.Success);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return Result<Guid>.Fail($"فشل في إضافة الرسالة: {ex.Message}", "AddMessageAggregateFailed", ResultStatus.Failed, ex);
            }
        }
    }
} 