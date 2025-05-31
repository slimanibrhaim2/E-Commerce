using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.DeleteMedia;

public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, Result<bool>>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMediaCommandHandler(IMediaRepository mediaRepository, IUnitOfWork unitOfWork)
    {
        _mediaRepository = mediaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _mediaRepository.DeleteMediaAsync(request.Id);
            await _unitOfWork.SaveChangesAsync();
            if (!deleted)
            {
                return Result<bool>.Fail(
                    message: "الوسائط غير موجودة أو فشل الحذف",
                    errorType: "MediaNotFoundOrDeleteFailed",
                    resultStatus: ResultStatus.NotFound);
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم حذف الوسائط بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في حذف الوسائط: {ex.Message}",
                errorType: "DeleteMediaFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 