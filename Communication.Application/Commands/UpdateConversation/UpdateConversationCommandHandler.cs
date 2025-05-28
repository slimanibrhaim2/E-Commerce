using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateConversation;

public class UpdateConversationCommandHandler : IRequestHandler<UpdateConversationCommand, Result<bool>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateConversationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.Id);
            if (conversation == null)
            {
                return Result<bool>.Fail(
                    message: "Conversation not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            if (string.IsNullOrWhiteSpace(request.Conversation.Title))
            {
                return Result<bool>.Fail(
                    message: "Title is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            conversation.Title = request.Conversation.Title;
            conversation.UpdatedAt = DateTime.UtcNow;

            _conversationRepository.Update(conversation);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: true,
                message: "Conversation updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to update conversation: {ex.Message}",
                errorType: "UpdateConversationFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 