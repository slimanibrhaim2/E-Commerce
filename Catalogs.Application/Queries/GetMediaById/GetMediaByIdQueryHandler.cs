using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Catalogs.Domain.Entities;

namespace Catalogs.Application.Queries.GetMediaById;

public class GetMediaByIdQueryHandler : IRequestHandler<GetMediaByIdQuery, Result<MediaDTO>>
{
    private readonly IMediaRepository _mediaRepo;
    private readonly ILogger<GetMediaByIdQueryHandler> _logger;

    public GetMediaByIdQueryHandler(IMediaRepository mediaRepo, ILogger<GetMediaByIdQueryHandler> logger)
    {
        _mediaRepo = mediaRepo;
        _logger = logger;
    }

    public async Task<Result<MediaDTO>> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Media? media = await _mediaRepo.GetByIdAsync(request.Id);
            if (media == null)
                return Result<MediaDTO>.Fail(
                    message: "الوسائط غير موجودة",
                    errorType: "MediaNotFound",
                    resultStatus: ResultStatus.NotFound);

            var mediaDto = new MediaDTO
            {
                Id = media.Id,
                Url = media.MediaUrl,
                MediaTypeId = media.MediaTypeId,
                BaseItemId = media.BaseItemId,
                // Add other properties as needed
            };

            return Result<MediaDTO>.Ok(
                data: mediaDto,
                message: "تم جلب الوسائط بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving media with ID {MediaId}", request.Id);
            return Result<MediaDTO>.Fail(
                message: "حدث خطأ أثناء جلب الوسائط",
                errorType: "GetMediaFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 