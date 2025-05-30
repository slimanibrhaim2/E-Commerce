using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateMessage;

public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, Result<bool>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMessageCommandHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var message = await _messageRepository.GetByIdAsync(request.Id);
            if (message == null)
            {
                return Result<bool>.Fail(
                    message: "Message not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            if (request.Message.ConversationId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "ConversationId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Message.SenderId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "SenderId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Message.BaseContentId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "BaseContentId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            message.ConversationId = request.Message.ConversationId;
            message.SenderId = request.Message.SenderId;
            message.BaseContentId = request.Message.BaseContentId;
            message.UpdatedAt = DateTime.UtcNow;

            _messageRepository.Update(message);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: true,
                message: "Message updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to update message: {ex.Message}",
                errorType: "UpdateMessageFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 