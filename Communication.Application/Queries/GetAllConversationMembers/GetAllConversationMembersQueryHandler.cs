using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllConversationMembers;

public class GetAllConversationMembersQueryHandler : IRequestHandler<GetAllConversationMembersQuery, Result<PaginatedResult<ConversationMemberDTO>>>
{
    private readonly IConversationMemberRepository _repository;

    public GetAllConversationMembersQueryHandler(IConversationMemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResult<ConversationMemberDTO>>> Handle(GetAllConversationMembersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var totalCount = entities.Count();
            var data = entities
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .Select(e => new ConversationMemberDTO
                {
                    Id = e.Id,
                    ConversationId = e.ConversationId,
                    UserId = e.UserId,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    DeletedAt = e.DeletedAt
                })
                .ToList();
            var paginated = Core.Pagination.PaginatedResult<ConversationMemberDTO>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<ConversationMemberDTO>>.Ok(
                paginated,
                message: "Conversation members retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<ConversationMemberDTO>>.Fail(
                message: $"Failed to get conversation members: {ex.Message}",
                errorType: "GetAllConversationMembersFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 