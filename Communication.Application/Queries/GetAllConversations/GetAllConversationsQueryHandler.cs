using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Communication.Application.Queries.GetAllConversations;

public class GetAllConversationsQueryHandler : IRequestHandler<GetAllConversationsQuery, Result<List<ConversationDTO>>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetAllConversationsQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<Result<List<ConversationDTO>>> Handle(GetAllConversationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _conversationRepository.GetAllAsync();
            var dtos = entities.Select(e => new ConversationDTO
            {
                Id = e.Id,
                Title = e.Title,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                DeletedAt = e.DeletedAt
            }).ToList();

            return Result<List<ConversationDTO>>.Ok(
                data: dtos,
                message: "Conversations retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<ConversationDTO>>.Fail(
                message: $"Failed to get conversations: {ex.Message}",
                errorType: "GetAllConversationsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 