using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Queries.GetMediaTypeById;

public class GetMediaTypeByIdQueryHandler : IRequestHandler<GetMediaTypeByIdQuery, Result<MediaTypeDTO>>
{
    private readonly IMediaTypeRepository _mediaTypeRepository;

    public GetMediaTypeByIdQueryHandler(IMediaTypeRepository mediaTypeRepository)
    {
        _mediaTypeRepository = mediaTypeRepository;
    }

    public async Task<Result<MediaTypeDTO>> Handle(GetMediaTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var mt = await _mediaTypeRepository.GetByIdAsync(request.Id);
        if (mt == null)
        {
            return Result<MediaTypeDTO>.Fail(
                message: "نوع الوسائط غير موجود",
                errorType: "NotFound",
                resultStatus: ResultStatus.NotFound);
        }
        var dto = new MediaTypeDTO
        {
            Id = mt.Id,
            Name = mt.Name,
            CreatedAt = mt.CreatedAt,
            UpdatedAt = mt.UpdatedAt,
            DeletedAt = mt.DeletedAt
        };
        return Result<MediaTypeDTO>.Ok(
            data: dto,
            message: "تم جلب نوع الوسائط بنجاح",
            resultStatus: ResultStatus.Success);
    }
} 