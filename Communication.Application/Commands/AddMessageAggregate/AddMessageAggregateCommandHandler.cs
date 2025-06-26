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
        private readonly IConversationMemberRepository _memberRepo;

        public AddMessageAggregateCommandHandler(
            IUnitOfWork unitOfWork,
            IMessageRepository messageRepo,
            IBaseContentRepository baseContentRepo,
            IAttachmentRepository attachmentRepo,
            IConversationRepository conversationRepo,
            IConversationMemberRepository memberRepo)
        {
            _unitOfWork = unitOfWork;
            _messageRepo = messageRepo;
            _baseContentRepo = baseContentRepo;
            _attachmentRepo = attachmentRepo;
            _conversationRepo = conversationRepo;
            _memberRepo = memberRepo;
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
                    // Create new conversation
                    conversation = new Conversation
                    {
                        Id = Guid.NewGuid(),
                        Title = dto.Content,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _conversationRepo.AddAsync(conversation);
                    await _unitOfWork.SaveChangesAsync();

                    // Add both users as members
                    var members = new[]
                    {
                        new ConversationMember
                        {
                            Id = Guid.NewGuid(),
                            ConversationId = conversation.Id,
                            UserId = dto.SenderId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        },
                        new ConversationMember
                        {
                            Id = Guid.NewGuid(),
                            ConversationId = conversation.Id,
                            UserId = dto.ReceiverId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }
                    };

                    foreach (var member in members)
                    {
                        await _memberRepo.AddAsync(member);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    // Verify both users are members
                    var members = await _memberRepo.GetAllAsync();
                    var conversationMembers = members.Where(m => m.ConversationId == conversation.Id).ToList();
                    var isSenderMember = conversationMembers.Any(m => m.UserId == dto.SenderId);
                    var isReceiverMember = conversationMembers.Any(m => m.UserId == dto.ReceiverId);

                    if (!isSenderMember || !isReceiverMember)
                    {
                        return Result<Guid>.Fail(
                            message: "One or both users are not members of this conversation",
                            errorType: "ValidationError",
                            resultStatus: ResultStatus.ValidationError);
                    }
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
                if (dto.Attachments != null)
                {
                    foreach (var attachmentDto in dto.Attachments)
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
                }

                await _unitOfWork.CommitTransaction();
                return Result<Guid>.Ok(
                    data: message.Id,
                    message: "تم إنشاء الرسالة بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return Result<Guid>.Fail(
                    message: $"فشل في إنشاء الرسالة: {ex.Message}",
                    errorType: "CreateMessageFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 