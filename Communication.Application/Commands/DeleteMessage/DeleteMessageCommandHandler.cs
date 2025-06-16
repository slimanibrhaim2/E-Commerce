using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.DeleteMessage;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Result<bool>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMessageCommandHandler(IMessageRepository messageRepository, IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var message = await _messageRepository.GetByIdAsync(request.Id);
            if (message == null)
            {
                return Result<bool>.Fail(
                    message: "الرسالة غير موجودة",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            _messageRepository.Remove(message);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: true,
                message: "تم حذف الرسالة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في حذف الرسالة: {ex.Message}",
                errorType: "DeleteMessageFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 