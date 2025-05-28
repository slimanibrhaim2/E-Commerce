using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteConversationMember;

public class DeleteConversationMemberCommandHandler : IRequestHandler<DeleteConversationMemberCommand, Result<bool>>
{
    private readonly IConversationMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteConversationMemberCommandHandler(IConversationMemberRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteConversationMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "Conversation member not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            _repository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Conversation member deleted successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to delete conversation member: {ex.Message}",
                errorType: "DeleteConversationMemberFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 