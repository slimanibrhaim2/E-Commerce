using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;

namespace Catalogs.Application.Queries.GetServicesByCategory;

public class GetServicesByCategoryQueryHandler : IRequestHandler<GetServicesByCategoryQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServicesByCategoryQueryHandler> _logger;

    public GetServicesByCategoryQueryHandler(
        IServiceRepository repo,
        ILogger<GetServicesByCategoryQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.CategoryId == Guid.Empty)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Category ID is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var services = (await _repo.GetByCategory(request.CategoryId)).ToList();
            
            if (!services.Any())
            {
                return Result<PaginatedResult<ServiceDTO>>.Ok(
                    data: PaginatedResult<ServiceDTO>.Create(
                        data: new List<ServiceDTO>(),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        totalCount: 0),
                    message: $"No services found in category ID {request.CategoryId}",
                    resultStatus: ResultStatus.Success);
            }

            var totalCount = services.Count;
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;
            var paged = services.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(s => new ServiceDTO
            {
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                ServiceType = s.ServiceType,
                Duration = s.Duration,
                IsAvailable = s.IsAvailable,
                CategoryName = s.Category?.Name,
                Media = s.Media?.Select(m => new MediaDTO
                {
                    Url = m.MediaUrl,
                    MediaTypeName = m.MediaType?.Name
                }).ToList() ?? new List<MediaDTO>(),
                Features = s.Features?.Select(f => new ServiceFeatureDTO
                {
                    Name = f.Name,
                    Value = f.Value
                }).ToList() ?? new List<ServiceFeatureDTO>()
            }).ToList();

            var paginated = PaginatedResult<ServiceDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} services from category ID {request.CategoryId}",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving services for category ID {CategoryId}", request.CategoryId);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "Failed to retrieve services due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving services for category ID {CategoryId}: {Message}", request.CategoryId, ex.Message);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "An unexpected error occurred while retrieving services. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 