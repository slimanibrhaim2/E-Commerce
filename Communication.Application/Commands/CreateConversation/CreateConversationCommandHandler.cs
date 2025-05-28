using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.CreateConversation;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<Guid>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateConversationCommandHandler(IConversationRepository conversationRepository, IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Conversation.Title))
        {
            return Result<Guid>.Fail(
                message: "Title is required",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var conversation = new Conversation
            {
                Title = request.Conversation.Title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add the conversation through repository (implement AddAsync in repo if not present)
            await _conversationRepository.AddAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(
                data: conversation.Id,
                message: "Conversation created successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to create conversation: {ex.Message}",
                errorType: "CreateConversationFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 