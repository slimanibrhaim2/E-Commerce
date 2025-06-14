using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetServiceById;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Result<ServiceDTO>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServiceByIdQueryHandler> _logger;

    public GetServiceByIdQueryHandler(
        IServiceRepository repo,
        ILogger<GetServiceByIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<ServiceDTO>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                return Result<ServiceDTO>.Fail(
                    message: "Service ID is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var service = await _repo.GetByIdAsync(request.Id);
            
            if (service == null)
            {
                return Result<ServiceDTO>.Fail(
                    message: $"Service with ID {request.Id} not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var dto = new ServiceDTO
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
                    MediaType = m.MediaType != null ? new MediaTypeDTO
                    {
                        Id = m.MediaType.Id,
                        Name = m.MediaType.Name,
                        CreatedAt = m.MediaType.CreatedAt,
                        UpdatedAt = m.MediaType.UpdatedAt
                    } : null,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList() ?? new List<MediaDTO>(),
                Features = service.Features?.Select(f => new ServiceFeatureDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value,
                    ServiceId = service.Id,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList() ?? new List<ServiceFeatureDTO>()
            };

            return Result<ServiceDTO>.Ok(
                data: dto,
                message: $"Successfully retrieved service '{service.Name}'",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving service with ID {ServiceId}", request.Id);
            return Result<ServiceDTO>.Fail(
                message: "Failed to retrieve service due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving service with ID {ServiceId}: {Message}", request.Id, ex.Message);
            return Result<ServiceDTO>.Fail(
                message: "An unexpected error occurred while retrieving the service. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 