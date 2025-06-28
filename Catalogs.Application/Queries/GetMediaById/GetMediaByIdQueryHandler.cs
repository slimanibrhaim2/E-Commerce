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
                    message: "Media not found",
                    errorType: "MediaNotFound",
                    resultStatus: ResultStatus.NotFound);

            var mediaDto = new MediaDTO
            {
                Url = media.MediaUrl,
                MediaTypeName = media.MediaType?.Name
            };

            return Result<MediaDTO>.Ok(
                data: mediaDto,
                message: "Media retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving media with ID {MediaId}", request.Id);
            return Result<MediaDTO>.Fail(
                message: "An error occurred while retrieving the media",
                errorType: "GetMediaFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 