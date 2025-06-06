using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetServiceById;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Result<ServiceDTO>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServiceByIdQueryHandler> _logger;

    public GetServiceByIdQueryHandler(IServiceRepository repo, ILogger<GetServiceByIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<ServiceDTO>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var service = await _repo.GetById(request.Id);
            if (service == null)
                return Result<ServiceDTO>.Fail(
                    message: "الخدمة غير موجودة",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);

            var serviceDto = new ServiceDTO
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                CategoryId = service.CategoryId,
                ServiceType = service.ServiceType,
                Duration = service.Duration,
                IsAvailable = service.IsAvailable,
                // Add other properties as needed
            };

            return Result<ServiceDTO>.Ok(
                data: serviceDto,
                message: "تم جلب الخدمة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service by ID");
            return Result<ServiceDTO>.Fail(
                message: "حدث خطأ أثناء جلب الخدمة",
                errorType: "GetServiceByIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 