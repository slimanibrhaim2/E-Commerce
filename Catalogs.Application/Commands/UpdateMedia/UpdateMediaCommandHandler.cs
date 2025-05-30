using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.UpdateMedia;

public class UpdateMediaCommandHandler : IRequestHandler<UpdateMediaCommand, Result<bool>>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMediaCommandHandler(IMediaRepository mediaRepository, IUnitOfWork unitOfWork)
    {
        _mediaRepository = mediaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateMediaCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Media.Url))
        {
            return Result<bool>.Fail(
                message: "رابط الوسائط مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (request.Media.MediaTypeId == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف نوع الوسائط مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var updated = await _mediaRepository.UpdateMediaAsync(
                request.Id,
                request.Media.Url,
                request.Media.MediaTypeId);
            await _unitOfWork.SaveChangesAsync();

            if (!updated)
            {
                return Result<bool>.Fail(
                    message: "لم يتم العثور على الوسائط أو فشل التحديث",
                    errorType: "MediaNotFoundOrUpdateFailed",
                    resultStatus: ResultStatus.NotFound);
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث الوسائط بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في تحديث الوسائط: {ex.Message}",
                errorType: "UpdateMediaFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 