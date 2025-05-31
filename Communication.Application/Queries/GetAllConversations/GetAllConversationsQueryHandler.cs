using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllConversations;

public class GetAllConversationsQueryHandler : IRequestHandler<GetAllConversationsQuery, Result<PaginatedResult<ConversationDTO>>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetAllConversationsQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<Result<PaginatedResult<ConversationDTO>>> Handle(GetAllConversationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _conversationRepository.GetAllAsync();
            var totalCount = entities.Count();
            var data = entities
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .Select(e => new ConversationDTO
                {
                    Id = e.Id,
                    Title = e.Title,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    DeletedAt = e.DeletedAt
                })
                .ToList();
            var paginated = Core.Pagination.PaginatedResult<ConversationDTO>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<ConversationDTO>>.Ok(
                paginated,
                message: "Conversations retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<ConversationDTO>>.Fail(
                message: $"Failed to get conversations: {ex.Message}",
                errorType: "GetAllConversationsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 