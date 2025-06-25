using MediatR;
using Core.Result;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Catalogs.Application.Queries.GetItemDetailsByBaseItemId
{
    public class GetItemDetailsByBaseItemIdQueryHandler : IRequestHandler<GetItemDetailsByBaseItemIdQuery, Result<ItemDetailsDTO>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<GetItemDetailsByBaseItemIdQueryHandler> _logger;

        public GetItemDetailsByBaseItemIdQueryHandler(
            IProductRepository productRepository,
            IServiceRepository serviceRepository,
            IMediator mediator,
            ILogger<GetItemDetailsByBaseItemIdQueryHandler> logger)
        {
            _productRepository = productRepository;
            _serviceRepository = serviceRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result<ItemDetailsDTO>> Handle(GetItemDetailsByBaseItemIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting item details for BaseItemId {BaseItemId}", request.BaseItemId);

                // First, try to find if it's a product
                var productId = await _productRepository.GetProductIdByBaseItemIdAsync(request.BaseItemId);
                if (productId.HasValue)
                {
                    var productQuery = new GetProductsByIdsQuery(new[] { request.BaseItemId }, new Core.Pagination.PaginationParameters { PageNumber = 1, PageSize = 1 });
                    var productResult = await _mediator.Send(productQuery, cancellationToken);
                    
                    if (productResult.Success && productResult.Data.Data.Any())
                    {
                        return Result<ItemDetailsDTO>.Ok(productResult.Data.Data.First(), "Product details retrieved successfully", ResultStatus.Success);
                    }
                }

                // If not a product, try to find if it's a service
                var serviceId = await _serviceRepository.GetServiceIdByBaseItemIdAsync(request.BaseItemId);
                if (serviceId.HasValue)
                {
                    var serviceQuery = new GetServicesByIdsQuery(new[] { request.BaseItemId }, new Core.Pagination.PaginationParameters { PageNumber = 1, PageSize = 1 });
                    var serviceResult = await _mediator.Send(serviceQuery, cancellationToken);
                    
                    if (serviceResult.Success && serviceResult.Data.Data.Any())
                    {
                        return Result<ItemDetailsDTO>.Ok(serviceResult.Data.Data.First(), "Service details retrieved successfully", ResultStatus.Success);
                    }
                }

                return Result<ItemDetailsDTO>.Fail(
                    message: $"Item with BaseItemId {request.BaseItemId} not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item details for BaseItemId {BaseItemId}", request.BaseItemId);
                return Result<ItemDetailsDTO>.Fail(
                    message: "Failed to retrieve item details",
                    errorType: "GetItemDetailsFailed",
                    resultStatus: ResultStatus.Failed);
            }
        }
    }
} 