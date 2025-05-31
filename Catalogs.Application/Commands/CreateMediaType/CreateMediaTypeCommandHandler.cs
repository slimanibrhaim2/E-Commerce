using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.CreateMediaType;

public class CreateMediaTypeCommandHandler : IRequestHandler<CreateMediaTypeCommand, Result<Guid>>
{
    private readonly IMediaTypeRepository _mediaTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMediaTypeCommandHandler(IMediaTypeRepository mediaTypeRepository, IUnitOfWork unitOfWork)
    {
        _mediaTypeRepository = mediaTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateMediaTypeCommand request, CancellationToken cancellationToken)
    {
        // Inline validation
        if (request.MediaType == null)
        {
            return Result<Guid>.Fail(
                message: "بيانات نوع الوسائط مطلوبة",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (string.IsNullOrWhiteSpace(request.MediaType.Name))
        {
            return Result<Guid>.Fail(
                message: "اسم نوع الوسائط مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        var mediaType = new MediaType
        {
            Name = request.MediaType.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var id = await _mediaTypeRepository.AddAsync(mediaType);
        await _unitOfWork.SaveChangesAsync();
        return Result<Guid>.Ok(
            data: id,
            message: "تم إنشاء نوع الوسائط بنجاح",
            resultStatus: ResultStatus.Success);
    }
} 