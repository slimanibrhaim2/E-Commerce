using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;

namespace Catalogs.Application.Queries.GetServicesByIds;

public class GetServicesByIdsQueryHandler : IRequestHandler<GetServicesByIdsQuery, Result<PaginatedResult<ServiceDetailsDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServicesByIdsQueryHandler> _logger;

    public GetServicesByIdsQueryHandler(
        IServiceRepository repo,
        ILogger<GetServicesByIdsQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ServiceDetailsDTO>>> Handle(GetServicesByIdsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ServiceIds == null || !request.ServiceIds.Any())
            {
                return Result<PaginatedResult<ServiceDetailsDTO>>.Fail(
                    message: "Service IDs list cannot be empty",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.ServiceIds.Any(id => id == Guid.Empty))
            {
                return Result<PaginatedResult<ServiceDetailsDTO>>.Fail(
                    message: "All service IDs must be valid GUIDs",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDetailsDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDetailsDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var services = (await _repo.GetByIdsAsync(request.ServiceIds)).ToList();
            
            if (!services.Any())
            {
                return Result<PaginatedResult<ServiceDetailsDTO>>.Ok(
                    data: PaginatedResult<ServiceDetailsDTO>.Create(
                        data: new List<ServiceDetailsDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: $"No services found for the provided IDs: {string.Join(", ", request.ServiceIds)}",
                    resultStatus: ResultStatus.Success);
            }

            var totalCount = services.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = services.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(s => new ServiceDetailsDTO
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
                Media = s.Media?.Select(m => new Shared.Contracts.DTOs.MediaDTO
                {
                    Id = m.Id,
                    Url = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    ItemId = m.BaseItemId,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList() ?? new List<Shared.Contracts.DTOs.MediaDTO>()
            }).ToList();

            var paginated = PaginatedResult<ServiceDetailsDTO>.Create(
                data: dtos.ToList(),
                pageNumber: pageNumber,
                pageSize: pageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDetailsDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} services out of {request.ServiceIds.ToList().Count} requested IDs",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving services for IDs: {ServiceIds}", string.Join(", ", request.ServiceIds));
            return Result<PaginatedResult<ServiceDetailsDTO>>.Fail(
                message: "Failed to retrieve services due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving services for IDs: {ServiceIds}: {Message}", 
                string.Join(", ", request.ServiceIds), ex.Message);
            return Result<PaginatedResult<ServiceDetailsDTO>>.Fail(
                message: "An unexpected error occurred while retrieving services. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 