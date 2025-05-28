using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Communication.Application.Queries.GetAllMessages;

public class GetAllMessagesQueryHandler : IRequestHandler<GetAllMessagesQuery, Result<List<MessageDTO>>>
{
    private readonly IMessageRepository _messageRepository;

    public GetAllMessagesQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result<List<MessageDTO>>> Handle(GetAllMessagesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _messageRepository.GetAllAsync();
            var dtos = entities.Select(e => new MessageDTO
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
            }).ToList();

            return Result<List<MessageDTO>>.Ok(
                data: dtos,
                message: "Messages retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<MessageDTO>>.Fail(
                message: $"Failed to get messages: {ex.Message}",
                errorType: "GetAllMessagesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 