using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByDuration;

public class GetServicesByDurationQueryHandler : IRequestHandler<GetServicesByDurationQuery, Result<PaginatedResult<ServiceDto>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServicesByDurationQueryHandler> _logger;

    public GetServicesByDurationQueryHandler(IServiceRepository repo, ILogger<GetServicesByDurationQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ServiceDto>>> Handle(GetServicesByDurationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MinDuration > request.MaxDuration)
                return Result<PaginatedResult<ServiceDto>>.Fail(
                    message: "المدة الأدنى يجب أن تكون أقل من المدة الأقصى",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);

            var services = await _repo.GetByDurationRange(request.MinDuration, request.MaxDuration);
            var serviceDtos = services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CategoryId = s.CategoryId,
                ServiceType = s.ServiceType,
                Duration = s.Duration,
                IsAvailable = s.IsAvailable,
                // Add other properties as needed
            }).ToList();
            var totalCount = serviceDtos.Count;

            var paginatedServices = PaginatedResult<ServiceDto>.Create(
                data: serviceDtos,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDto>>.Ok(
                data: paginatedServices,
                message: "تم جلب الخدمات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving services in duration range {MinDuration} - {MaxDuration}", request.MinDuration, request.MaxDuration);
            return Result<PaginatedResult<ServiceDto>>.Fail(
                message: "حدث خطأ أثناء جلب الخدمات",
                errorType: "GetServicesByDurationFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 