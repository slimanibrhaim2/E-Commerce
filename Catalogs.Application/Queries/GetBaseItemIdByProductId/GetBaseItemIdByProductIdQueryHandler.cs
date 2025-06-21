using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Shared.Contracts.Queries;

namespace Catalogs.Application.Queries.GetBaseItemIdByProductId;

public class GetBaseItemIdByProductIdQueryHandler : IRequestHandler<GetBaseItemIdByProductIdQuery, Result<Guid>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetBaseItemIdByProductIdQueryHandler> _logger;

    public GetBaseItemIdByProductIdQueryHandler(IProductRepository repo, ILogger<GetBaseItemIdByProductIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(GetBaseItemIdByProductIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ProductId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "Product ID is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var baseItemId = await _repo.GetBaseItemIdByProductIdAsync(request.ProductId);
            if (baseItemId == null)
            {
                return Result<Guid>.Fail(
                    message: $"Product with ID {request.ProductId} not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            return Result<Guid>.Ok(
                data: baseItemId.Value,
                message: $"Successfully retrieved base item ID for product {request.ProductId}",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving base item ID for product with ID {ProductId}", request.ProductId);
            return Result<Guid>.Fail(
                message: "Failed to retrieve base item ID due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving base item ID for product with ID {ProductId}: {Message}", request.ProductId, ex.Message);
            return Result<Guid>.Fail(
                message: "An unexpected error occurred while retrieving the base item ID. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 