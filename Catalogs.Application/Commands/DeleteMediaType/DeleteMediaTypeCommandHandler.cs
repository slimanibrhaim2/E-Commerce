using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.DeleteMediaType;

public class DeleteMediaTypeCommandHandler : IRequestHandler<DeleteMediaTypeCommand, Result<bool>>
{
    private readonly IMediaTypeRepository _mediaTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMediaTypeCommandHandler(IMediaTypeRepository mediaTypeRepository, IUnitOfWork unitOfWork)
    {
        _mediaTypeRepository = mediaTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteMediaTypeCommand request, CancellationToken cancellationToken)
    {
        // Inline validation
        if (request.Id == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف نوع الوسائط مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        var deleted = await _mediaTypeRepository.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync();
        if (!deleted)
        {
            return Result<bool>.Fail(
                message: "لم يتم العثور على نوع الوسائط أو فشل الحذف",
                errorType: "MediaTypeNotFoundOrDeleteFailed",
                resultStatus: ResultStatus.NotFound);
        }

        return Result<bool>.Ok(
            data: true,
            message: "تم حذف نوع الوسائط بنجاح",
            resultStatus: ResultStatus.Success);
    }
} 