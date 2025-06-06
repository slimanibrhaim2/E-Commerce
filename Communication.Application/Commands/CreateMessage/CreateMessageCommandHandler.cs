using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.CreateMessage;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Result<Guid>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMessageCommandHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Message.ConversationId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "ConversationId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Message.SenderId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "SenderId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Message.BaseContentId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "BaseContentId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var message = new Message
            {
                ConversationId = request.Message.ConversationId,
                SenderId = request.Message.SenderId,
                BaseContentId = request.Message.BaseContentId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(
                data: message.Id,
                message: "Message created successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to create message: {ex.Message}",
                errorType: "CreateMessageFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 