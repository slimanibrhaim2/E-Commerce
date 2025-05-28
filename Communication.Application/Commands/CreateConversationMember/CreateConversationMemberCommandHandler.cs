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
    private readonly IUnitOfWork _unitOfWork;

    public CreateConversationMemberCommandHandler(IConversationMemberRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
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
            var entity = new ConversationMember
            {
                ConversationId = request.ConversationMember.ConversationId,
                UserId = request.ConversationMember.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(
                data: entity.Id,
                message: "Conversation member created successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to create conversation member: {ex.Message}",
                errorType: "CreateConversationMemberFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 