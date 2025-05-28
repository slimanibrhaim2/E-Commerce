using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateConversationMember;

public class UpdateConversationMemberCommandHandler : IRequestHandler<UpdateConversationMemberCommand, Result<bool>>
{
    private readonly IConversationMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateConversationMemberCommandHandler(IConversationMemberRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateConversationMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ConversationMember.ConversationId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "ConversationId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.ConversationMember.UserId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "UserId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "Conversation member not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            entity.ConversationId = request.ConversationMember.ConversationId;
            entity.UserId = request.ConversationMember.UserId;
            entity.UpdatedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Conversation member updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to update conversation member: {ex.Message}",
                errorType: "UpdateConversationMemberFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 