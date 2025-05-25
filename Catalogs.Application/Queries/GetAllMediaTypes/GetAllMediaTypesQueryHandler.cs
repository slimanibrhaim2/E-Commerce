using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Queries.GetAllMediaTypes;

public class GetAllMediaTypesQueryHandler : IRequestHandler<GetAllMediaTypesQuery, Result<List<MediaTypeDTO>>>
{
    private readonly IMediaTypeRepository _mediaTypeRepository;

    public GetAllMediaTypesQueryHandler(IMediaTypeRepository mediaTypeRepository)
    {
        _mediaTypeRepository = mediaTypeRepository;
    }

    public async Task<Result<List<MediaTypeDTO>>> Handle(GetAllMediaTypesQuery request, CancellationToken cancellationToken)
    {
        var mediaTypes = await _mediaTypeRepository.GetAllAsync();
        var dtos = mediaTypes.Select(mt => new MediaTypeDTO
        {
            Id = mt.Id,
            Name = mt.Name,
            CreatedAt = mt.CreatedAt,
            UpdatedAt = mt.UpdatedAt,
            DeletedAt = mt.DeletedAt
        }).ToList();
        return Result<List<MediaTypeDTO>>.Ok(
            data: dtos,
            message: "تم جلب أنواع الوسائط بنجاح",
            resultStatus: ResultStatus.Success);
    }
} 