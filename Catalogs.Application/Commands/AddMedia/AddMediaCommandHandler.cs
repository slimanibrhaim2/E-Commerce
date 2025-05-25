using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Commands.AddMedia;

public class AddMediaCommandHandler : IRequestHandler<AddMediaCommand, Result<Guid>>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly ILogger<AddMediaCommandHandler> _logger;

    public AddMediaCommandHandler(
        IMediaRepository mediaRepository,
        ILogger<AddMediaCommandHandler> logger)
    {
        _mediaRepository = mediaRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(AddMediaCommand request, CancellationToken cancellationToken)
    {
        // Use validator class
        var validator = new AddMediaCommandValidator();
        var validationResult = validator.Validate(request);
        if (!validationResult.Success)
        {
            return Result<Guid>.Fail(
                message: validationResult.Message,
                errorType: validationResult.ErrorType,
                resultStatus: validationResult.ResultStatus);
        }

        try
        {
            var mediaId = await _mediaRepository.AddMediaAsync(request.ItemId, request.Mediadto.Url,request.Mediadto.MediaTypeId);
            if (mediaId.HasValue)
            {
                return Result<Guid>.Ok(
                    data: mediaId.Value,
                    message: "تم إضافة الوسائط بنجاح",
                    resultStatus: ResultStatus.Success);
            }

            return Result<Guid>.Fail(
                message: "لم يتم العثور على المنتج أو الخدمة",
                errorType: "ItemNotFound",
                resultStatus: ResultStatus.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding media");
            return Result<Guid>.Fail(
                message: "حدث خطأ أثناء إضافة الوسائط",
                errorType: "AddMediaFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 