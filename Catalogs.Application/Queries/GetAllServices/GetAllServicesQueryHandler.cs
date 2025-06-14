using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;
using System.Data;

namespace Catalogs.Application.Queries.GetAllServices;

public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetAllServicesQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;

    public GetAllServicesQueryHandler(
        IServiceRepository repo, 
        ILogger<GetAllServicesQueryHandler> logger,
        IFeatureRepository featureRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Pagination.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Pagination.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var services = await _repo.GetAllAsync();
            if (!services.Any())
            {
                return Result<PaginatedResult<ServiceDTO>>.Ok(
                    data: PaginatedResult<ServiceDTO>.Create(
                        data: new List<ServiceDTO>(),
                        pageNumber: request.Pagination.PageNumber,
                        pageSize: request.Pagination.PageSize,
                        totalCount: 0),
                    message: "No services found",
                    resultStatus: ResultStatus.Success);
            }

            var serviceDtos = new List<ServiceDTO>();

            foreach (var service in services)
            {
                try
                {
                    var features = await _featureRepo.GetServiceFeaturesByEntityIdAsync(service.Id);
                    serviceDtos.Add(new ServiceDTO
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Description = service.Description,
                        Price = service.Price,
                        CategoryId = service.CategoryId,
                        ServiceType = service.ServiceType,
                        Duration = service.Duration,
                        IsAvailable = service.IsAvailable,
                        UserId = service.UserId,
                        CreatedAt = service.CreatedAt,
                        UpdatedAt = service.UpdatedAt,
                        Media = service.Media?.Select(m => new MediaDTO
                        {
                            Id = m.Id,
                            Url = m.MediaUrl,
                            MediaTypeId = m.MediaTypeId,
                            ItemId = m.BaseItemId,
                            CreatedAt = m.CreatedAt,
                            UpdatedAt = m.UpdatedAt
                        }).ToList() ?? new List<MediaDTO>(),
                        Features = features.Select(f => new ServiceFeatureDTO
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Value = f.Value,
                            ServiceId = f.ServiceId,
                            CreatedAt = f.CreatedAt,
                            UpdatedAt = f.UpdatedAt
                        }).ToList()
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process service {ServiceId}: {Message}", service.Id, ex.Message);
                    // Continue processing other services even if one fails
                }
            }

            var totalCount = serviceDtos.Count;
            var paginatedServices = PaginatedResult<ServiceDTO>.Create(
                data: serviceDtos,
                pageNumber: request.Pagination.PageNumber,
                pageSize: request.Pagination.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginatedServices,
                message: $"Successfully retrieved {serviceDtos.Count} services out of {totalCount} total services",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving services");
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "Failed to retrieve services due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving services: {Message}", ex.Message);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "An unexpected error occurred while retrieving services. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 