using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Communication.Application.Queries.GetAllConversationMembers;

public class GetAllConversationMembersQueryHandler : IRequestHandler<GetAllConversationMembersQuery, Result<List<ConversationMemberDTO>>>
{
    private readonly IConversationMemberRepository _repository;

    public GetAllConversationMembersQueryHandler(IConversationMemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<ConversationMemberDTO>>> Handle(GetAllConversationMembersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var dtos = entities.Select(e => new ConversationMemberDTO
            {
                Id = e.Id,
                ConversationId = e.ConversationId,
                UserId = e.UserId,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                DeletedAt = e.DeletedAt
            }).ToList();
            return Result<List<ConversationMemberDTO>>.Ok(
                data: dtos,
                message: "Conversation members retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<ConversationMemberDTO>>.Fail(
                message: $"Failed to get conversation members: {ex.Message}",
                errorType: "GetAllConversationMembersFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 