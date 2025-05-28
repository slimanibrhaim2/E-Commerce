using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteConversation;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, Result<bool>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
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

            _conversationRepository.Remove(conversation);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: true,
                message: "Conversation deleted successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to delete conversation: {ex.Message}",
                errorType: "DeleteConversationFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 