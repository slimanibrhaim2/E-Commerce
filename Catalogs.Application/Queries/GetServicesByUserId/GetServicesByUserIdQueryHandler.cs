using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetServicesByUserId;

public class GetServicesByUserIdQueryHandler : IRequestHandler<GetServicesByUserIdQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repository;
    private readonly ILogger<GetServicesByUserIdQueryHandler> _logger;

    public GetServicesByUserIdQueryHandler(
        IServiceRepository repository,
        ILogger<GetServicesByUserIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.UserId == Guid.Empty)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "User ID is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var services = (await _repository.GetServicesByUserIdAsync(request.UserId)).ToList();
            
            if (!services.Any())
            {
                return Result<PaginatedResult<ServiceDTO>>.Ok(
                    data: PaginatedResult<ServiceDTO>.Create(
                        data: new List<ServiceDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: $"No services found for user {request.UserId}",
                    resultStatus: ResultStatus.Success);
            }

            var totalCount = services.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = services.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(s => new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CategoryId = s.CategoryId,
                ServiceType = s.ServiceType,
                Duration = s.Duration,
                IsAvailable = s.IsAvailable,
                UserId = s.UserId,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                Media = s.Media?.Select(m => new MediaDTO
                {
                    Id = m.Id,
                    Url = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    BaseItemId = m.BaseItemId,
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
                Features = s.Features?.Select(f => new ServiceFeatureDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value,
                    ServiceId = s.Id,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList() ?? new List<ServiceFeatureDTO>()
            }).ToList();

            var paginated = PaginatedResult<ServiceDTO>.Create(
                data: dtos,
                pageNumber: pageNumber,
                pageSize: pageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} services for user {request.UserId} out of {totalCount} total services",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving services for user {UserId}", request.UserId);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "Failed to retrieve services due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving services for user {UserId}: {Message}", request.UserId, ex.Message);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "An unexpected error occurred while retrieving services. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 