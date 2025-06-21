using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Shared.Contracts.Queries;

namespace Catalogs.Application.Queries.GetBaseItemIdByServiceId;

public class GetBaseItemIdByServiceIdQueryHandler : IRequestHandler<GetBaseItemIdByServiceIdQuery, Result<Guid>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetBaseItemIdByServiceIdQueryHandler> _logger;

    public GetBaseItemIdByServiceIdQueryHandler(IServiceRepository repo, ILogger<GetBaseItemIdByServiceIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(GetBaseItemIdByServiceIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ServiceId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "Service ID is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var baseItemId = await _repo.GetBaseItemIdByServiceIdAsync(request.ServiceId);
            if (baseItemId == null)
            {
                return Result<Guid>.Fail(
                    message: $"Service with ID {request.ServiceId} not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            return Result<Guid>.Ok(
                data: baseItemId.Value,
                message: $"Successfully retrieved base item ID for service {request.ServiceId}",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving base item ID for service with ID {ServiceId}", request.ServiceId);
            return Result<Guid>.Fail(
                message: "Failed to retrieve base item ID due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving base item ID for service with ID {ServiceId}: {Message}", request.ServiceId, ex.Message);
            return Result<Guid>.Fail(
                message: "An unexpected error occurred while retrieving the base item ID. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 