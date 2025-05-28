using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;

namespace Communication.Application.Queries.GetConversationMemberById;

public class GetConversationMemberByIdQueryHandler : IRequestHandler<GetConversationMemberByIdQuery, Result<ConversationMemberDTO>>
{
    private readonly IConversationMemberRepository _repository;

    public GetConversationMemberByIdQueryHandler(IConversationMemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ConversationMemberDTO>> Handle(GetConversationMemberByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<ConversationMemberDTO>.Fail(
                    message: "Conversation member not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            var dto = new ConversationMemberDTO
            {
                Id = entity.Id,
                ConversationId = entity.ConversationId,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt
            };
            return Result<ConversationMemberDTO>.Ok(
                data: dto,
                message: "Conversation member retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<ConversationMemberDTO>.Fail(
                message: $"Failed to get conversation member: {ex.Message}",
                errorType: "GetConversationMemberFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 