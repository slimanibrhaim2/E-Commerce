using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllMessages;

public class GetAllMessagesQueryHandler : IRequestHandler<GetAllMessagesQuery, Result<PaginatedResult<MessageDTO>>>
{
    private readonly IMessageRepository _messageRepository;

    public GetAllMessagesQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result<PaginatedResult<MessageDTO>>> Handle(GetAllMessagesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _messageRepository.GetAllAsync();
            var totalCount = entities.Count();
            var data = entities
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .Select(e => new MessageDTO
                {
                    Id = e.Id,
                    ConversationId = e.ConversationId,
                    SenderId = e.SenderId,
                    BaseContentId = e.BaseContentId,
                    IsRead = e.IsRead,
                    ReadAt = e.ReadAt,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    DeletedAt = e.DeletedAt
                })
                .ToList();
            var paginated = Core.Pagination.PaginatedResult<MessageDTO>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<MessageDTO>>.Ok(
                paginated,
                message: "تم جلب الرسائل بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<MessageDTO>>.Fail(
                message: $"فشل في جلب الرسائل: {ex.Message}",
                errorType: "GetAllMessagesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 