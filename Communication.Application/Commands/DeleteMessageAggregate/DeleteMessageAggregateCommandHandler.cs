using MediatR;
using Communication.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteMessageAggregate;

public class DeleteMessageAggregateCommandHandler : IRequestHandler<DeleteMessageAggregateCommand, Result<bool>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMessageAggregateCommandHandler(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteMessageAggregateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get existing message
            var message = await _messageRepository.GetByIdAsync(request.Id);
            if (message == null)
            {
                return Result<bool>.Fail(
                    message: $"Message with ID {request.Id} not found.",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // 2. Soft delete message
            message.DeletedAt = DateTime.UtcNow;
            _messageRepository.Remove(message);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: true,
                message: "Message deleted successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to delete message: {ex.Message}",
                errorType: "DeleteMessageFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 