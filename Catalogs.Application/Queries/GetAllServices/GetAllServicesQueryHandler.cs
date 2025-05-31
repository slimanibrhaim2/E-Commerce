using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllServices;

public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetAllServicesQueryHandler> _logger;

    public GetAllServicesQueryHandler(IServiceRepository repo, ILogger<GetAllServicesQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var services = await _repo.GetAllAsync();
            var serviceDtos = services.Select(s => new ServiceDTO
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

            var paginatedServices = PaginatedResult<ServiceDTO>.Create(
                data: serviceDtos,
                pageNumber: request.Pagination.PageNumber,
                pageSize: request.Pagination.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginatedServices,
                message: "تم جلب الخدمات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all services");
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "حدث خطأ أثناء جلب الخدمات",
                errorType: "GetAllServicesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 