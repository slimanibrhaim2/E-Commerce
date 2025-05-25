using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Catalogs.Domain.Entities;

namespace Catalogs.Application.Commands.UpdateService.Simple;

public class UpdateServiceSimpleCommandHandler : IRequestHandler<UpdateServiceSimpleCommand, Result<bool>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<UpdateServiceSimpleCommandHandler> _logger;

    public UpdateServiceSimpleCommandHandler(IServiceRepository repo, ILogger<UpdateServiceSimpleCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateServiceSimpleCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.ServiceDto.Name))
        {
            return Result<bool>.Fail(
                message: "اسم الخدمة مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (request.ServiceDto.Price <= 0)
        {
            return Result<bool>.Fail(
                message: "سعر الخدمة يجب أن يكون أكبر من الصفر",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (request.ServiceDto.CategoryId == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف التصنيف مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var service = new Service
            {
                Id = request.Id,
                Name = request.ServiceDto.Name,
                Description = request.ServiceDto.Description,
                Price = request.ServiceDto.Price,
                CategoryId = request.ServiceDto.CategoryId,
                ServiceType = request.ServiceDto.ServiceType,
                Duration = request.ServiceDto.Duration,
                IsAvailable = request.ServiceDto.IsAvailable,
                // Add other properties as needed
            };

            var result = await _repo.UpdateAsync(request.Id, service);
            if (!result)
            {
                return Result<bool>.Fail(
                    message: "فشل في تحديث الخدمة",
                    errorType: "UpdateServiceFailed",
                    resultStatus: ResultStatus.Failed);
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث الخدمة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating service with ID {ServiceId}", request.Id);
            return Result<bool>.Fail(
                message: "حدث خطأ أثناء تحديث الخدمة",
                errorType: "UpdateServiceFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}