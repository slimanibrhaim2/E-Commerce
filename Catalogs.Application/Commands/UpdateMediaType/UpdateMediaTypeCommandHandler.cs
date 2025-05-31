using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.UpdateMediaType;

public class UpdateMediaTypeCommandHandler : IRequestHandler<UpdateMediaTypeCommand, Result<bool>>
{
    private readonly IMediaTypeRepository _mediaTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMediaTypeCommandHandler(IMediaTypeRepository mediaTypeRepository, IUnitOfWork unitOfWork)
    {
        _mediaTypeRepository = mediaTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateMediaTypeCommand request, CancellationToken cancellationToken)
    {
        // Inline validation
        if (request.Id == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف نوع الوسائط مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (request.MediaType == null)
        {
            return Result<bool>.Fail(
                message: "بيانات نوع الوسائط مطلوبة",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (string.IsNullOrWhiteSpace(request.MediaType.Name))
        {
            return Result<bool>.Fail(
                message: "اسم نوع الوسائط مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        var mediaType = new MediaType
        {
            Id = request.Id,
            Name = request.MediaType.Name,
            UpdatedAt = DateTime.UtcNow
        };

        var updated = await _mediaTypeRepository.UpdateAsync(mediaType);
        await _unitOfWork.SaveChangesAsync();
        if (!updated)
        {
            return Result<bool>.Fail(
                message: "لم يتم العثور على نوع الوسائط أو فشل التحديث",
                errorType: "MediaTypeNotFoundOrUpdateFailed",
                resultStatus: ResultStatus.NotFound);
        }

        return Result<bool>.Ok(
            data: true,
            message: "تم تحديث نوع الوسائط بنجاح",
            resultStatus: ResultStatus.Success);
    }
} 