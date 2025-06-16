using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;

namespace Communication.Application.Queries.GetMessageById;

public class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, Result<MessageDTO>>
{
    private readonly IMessageRepository _messageRepository;

    public GetMessageByIdQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result<MessageDTO>> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _messageRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<MessageDTO>.Fail(
                    message: "الرسالة غير موجودة",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            var dto = new MessageDTO
            {
                Id = entity.Id,
                ConversationId = entity.ConversationId,
                SenderId = entity.SenderId,
                BaseContentId = entity.BaseContentId,
                IsRead = entity.IsRead,
                ReadAt = entity.ReadAt,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt
            };
            return Result<MessageDTO>.Ok(
                data: dto,
                message: "تم جلب الرسالة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<MessageDTO>.Fail(
                message: $"فشل في جلب الرسالة: {ex.Message}",
                errorType: "GetMessageFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 