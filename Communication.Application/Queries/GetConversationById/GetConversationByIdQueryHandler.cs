using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;

namespace Communication.Application.Queries.GetConversationById;

public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, Result<ConversationDTO>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationByIdQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<Result<ConversationDTO>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.Id);
            if (conversation == null)
            {
                return Result<ConversationDTO>.Fail(
                    message: "Conversation not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var dto = new ConversationDTO
            {
                Id = conversation.Id,
                Title = conversation.Title,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt,
                DeletedAt = conversation.DeletedAt
            };

            return Result<ConversationDTO>.Ok(
                data: dto,
                message: "Conversation found successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<ConversationDTO>.Fail(
                message: $"Failed to get conversation: {ex.Message}",
                errorType: "GetConversationFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 