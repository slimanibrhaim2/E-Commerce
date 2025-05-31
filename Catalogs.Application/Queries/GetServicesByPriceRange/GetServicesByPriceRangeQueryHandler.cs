using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByPriceRange;

public class GetServicesByPriceRangeQueryHandler : IRequestHandler<GetServicesByPriceRangeQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServicesByPriceRangeQueryHandler> _logger;

    public GetServicesByPriceRangeQueryHandler(IServiceRepository repo, ILogger<GetServicesByPriceRangeQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MinPrice > request.MaxPrice)
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "السعر الأدنى يجب أن يكون أقل من السعر الأقصى",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);

            var services = await _repo.GetByPriceRange(request.MinPrice, request.MaxPrice);
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
            _logger.LogError(ex, "Error retrieving services in price range {MinPrice} - {MaxPrice}", request.MinPrice, request.MaxPrice);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "حدث خطأ أثناء جلب الخدمات",
                errorType: "GetServicesByPriceRangeFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 