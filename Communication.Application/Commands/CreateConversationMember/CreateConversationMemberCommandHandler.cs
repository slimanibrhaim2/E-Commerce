using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.CreateConversationMember;

public class CreateConversationMemberCommandHandler : IRequestHandler<CreateConversationMemberCommand, Result<Guid>>
{
    private readonly IConversationMemberRepository _repository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateConversationMemberCommandHandler(
        IConversationMemberRepository repository, 
        IConversationRepository conversationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateConversationMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ConversationMember.ConversationId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "ConversationId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.ConversationMember.UserId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "UserId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            await _unitOfWork.BeginTransaction();

            // Check if conversation exists, if not create it
            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationMember.ConversationId);
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    Id = request.ConversationMember.ConversationId,
                    Title = "New Conversation", // You might want to make this configurable
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _conversationRepository.AddAsync(conversation);
                await _unitOfWork.SaveChangesAsync();
            }

            // Check if member already exists
            var existingMembers = await _repository.GetAllAsync();
            var alreadyMember = existingMembers.Any(m => 
                m.ConversationId == request.ConversationMember.ConversationId && 
                m.UserId == request.ConversationMember.UserId);

            if (alreadyMember)
            {
                await _unitOfWork.RollbackTransaction();
                return Result<Guid>.Fail(
                    message: "User is already a member of this conversation",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var entity = new ConversationMember
            {
                ConversationId = request.ConversationMember.ConversationId,
                UserId = request.ConversationMember.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();

            return Result<Guid>.Ok(
                data: entity.Id,
                message: "Conversation member created successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<Guid>.Fail(
                message: $"Failed to create conversation member: {ex.Message}",
                errorType: "CreateConversationMemberFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 