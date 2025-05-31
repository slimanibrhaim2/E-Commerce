using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalogs.Application.Queries.GetServicesByName;

public class GetServicesByNameQueryHandler : IRequestHandler<GetServicesByNameQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repository;

    public GetServicesByNameQueryHandler(IServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var services = (await _repository.GetServicesByNameAsync(request.Name)).ToList();
            var totalCount = services.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = services.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var dtos = paged.Select(s => new ServiceDTO
            {
                Id = s.Id,
                ServiceType = s.ServiceType,
                Duration = s.Duration,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CategoryId = s.CategoryId,
                IsAvailable = s.IsAvailable,
                UserId = s.UserId
            }).ToList();
            var paginated = PaginatedResult<ServiceDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginated,
                message: "Services retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: $"Failed to get services: {ex.Message}",
                errorType: "GetServicesByNameFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 