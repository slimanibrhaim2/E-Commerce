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
using System.Collections.Generic;

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

                if (request.BaseItemId == Guid.Empty)
                {
                    return Result<ItemDetailsDTO>.Fail(
                        message: "معرف العنصر الأساسي مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // First, try to find if it's a product
                var productId = await _productRepository.GetProductIdByBaseItemIdAsync(request.BaseItemId);
                if (productId.HasValue)
                {
                    var product = await _productRepository.GetByIdWithDetails(productId.Value);
                    if (product != null)
                    {
                        var productDetails = new ProductDetailsDTO
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            CategoryId = product.CategoryId,
                            SKU = product.SKU,
                            SerialNumber = product.SerialNumber,
                            StockQuantity = product.StockQuantity,
                            IsAvailable = product.IsAvailable,
                            UserId = product.UserId,
                            CreatedAt = product.CreatedAt,
                            UpdatedAt = product.UpdatedAt,
                            Media = product.Media?.Select(m => new MediaDTO
                            {
                                Id = m.Id,
                                Url = m.MediaUrl,
                                MediaTypeId = m.MediaTypeId,
                                ItemId = m.BaseItemId,
                                CreatedAt = m.CreatedAt,
                                UpdatedAt = m.UpdatedAt
                            }).ToList() ?? new List<MediaDTO>(),
                            Features = product.Features?.Select(f => new ProductFeatureDTO
                            {
                                Id = f.Id,
                                Name = f.Name,
                                Value = f.Value,
                                ProductId = product.Id,
                                CreatedAt = f.CreatedAt,
                                UpdatedAt = f.UpdatedAt
                            }).ToList() ?? new List<ProductFeatureDTO>()
                        };
                        return Result<ItemDetailsDTO>.Ok(productDetails, "تم جلب تفاصيل المنتج بنجاح", ResultStatus.Success);
                    }
                }

                // If not a product, try to find if it's a service
                var serviceId = await _serviceRepository.GetServiceIdByBaseItemIdAsync(request.BaseItemId);
                if (serviceId.HasValue)
                {
                    var service = await _serviceRepository.GetByIdWithDetails(serviceId.Value);
                    if (service != null)
                    {
                        var serviceDetails = new ServiceDetailsDTO
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
                            }).ToList() ?? new List<MediaDTO>()
                        };
                        return Result<ItemDetailsDTO>.Ok(serviceDetails, "تم جلب تفاصيل الخدمة بنجاح", ResultStatus.Success);
                    }
                }

                return Result<ItemDetailsDTO>.Fail(
                    message: $"لم يتم العثور على عنصر بالمعرف الأساسي {request.BaseItemId}",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item details for BaseItemId {BaseItemId}", request.BaseItemId);
                return Result<ItemDetailsDTO>.Fail(
                    message: "فشل في جلب تفاصيل العنصر",
                    errorType: "GetItemDetailsFailed",
                    resultStatus: ResultStatus.Failed);
            }
        }
    }
} 