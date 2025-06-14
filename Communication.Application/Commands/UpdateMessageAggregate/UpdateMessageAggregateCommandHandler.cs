using MediatR;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateMessageAggregate;

public class UpdateMessageAggregateCommandHandler : IRequestHandler<UpdateMessageAggregateCommand, Result<Guid>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IBaseContentRepository _baseContentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMessageAggregateCommandHandler(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        IBaseContentRepository baseContentRepository,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _baseContentRepository = baseContentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateMessageAggregateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get existing message with its base content
            var message = await _messageRepository.GetByIdAsync(request.Id);
            if (message == null)
            {
                return Result<Guid>.Fail(
                    message: $"Message with ID {request.Id} not found.",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // 2. Get and update base content
            var baseContent = await _baseContentRepository.GetByIdAsync(message.BaseContentId);
            if (baseContent == null)
            {
                return Result<Guid>.Fail(
                    message: $"Base content for message {request.Id} not found.",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            baseContent.Title = request.DTO.Content;
            baseContent.Description = request.DTO.Content;
            baseContent.UpdatedAt = DateTime.UtcNow;

            // 3. Update message
            message.UpdatedAt = DateTime.UtcNow;

            // 4. Save changes
            _baseContentRepository.Remove(baseContent);
            _messageRepository.Remove(message);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(
                data: message.Id,
                message: "Message updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to update message: {ex.Message}",
                errorType: "UpdateMessageFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 