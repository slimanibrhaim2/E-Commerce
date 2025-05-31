using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByCategory;

public class GetServicesByCategoryQueryHandler : IRequestHandler<GetServicesByCategoryQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServicesByCategoryQueryHandler> _logger;

    public GetServicesByCategoryQueryHandler(IServiceRepository repo, ILogger<GetServicesByCategoryQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var services = await _repo.GetByCategory(request.CategoryId);
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
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginatedServices,
                message: "تم جلب الخدمات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving services by category {CategoryId}", request.CategoryId);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "حدث خطأ أثناء جلب الخدمات",
                errorType: "GetServicesByCategoryFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 