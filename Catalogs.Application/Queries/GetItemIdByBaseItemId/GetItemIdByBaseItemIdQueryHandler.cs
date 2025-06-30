using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using System;

namespace Catalogs.Application.Queries.GetItemIdByBaseItemId;

public class GetItemIdByBaseItemIdQueryHandler : IRequestHandler<GetItemIdByBaseItemIdQuery, Result<ItemIdResponseDTO>>
{
    private readonly IProductRepository _productRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly ILogger<GetItemIdByBaseItemIdQueryHandler> _logger;

    public GetItemIdByBaseItemIdQueryHandler(
        IProductRepository productRepo,
        IServiceRepository serviceRepo,
        ILogger<GetItemIdByBaseItemIdQueryHandler> logger)
    {
        _productRepo = productRepo;
        _serviceRepo = serviceRepo;
        _logger = logger;
    }

    public async Task<Result<ItemIdResponseDTO>> Handle(GetItemIdByBaseItemIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.BaseItemId == Guid.Empty)
            {
                return Result<ItemIdResponseDTO>.Fail(
                    message: "معرف العنصر الأساسي مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // First try to get product ID
            var productId = await _productRepo.GetProductIdByBaseItemIdAsync(request.BaseItemId);
            if (productId.HasValue)
            {
                return Result<ItemIdResponseDTO>.Ok(
                    data: new ItemIdResponseDTO 
                    { 
                        ItemId = productId.Value,
                        ItemType = "Product"
                    },
                    message: "تم العثور على معرف المنتج بنجاح",
                    resultStatus: ResultStatus.Success);
            }

            // If not a product, try to get service ID
            var serviceId = await _serviceRepo.GetServiceIdByBaseItemIdAsync(request.BaseItemId);
            if (serviceId.HasValue)
            {
                return Result<ItemIdResponseDTO>.Ok(
                    data: new ItemIdResponseDTO 
                    { 
                        ItemId = serviceId.Value,
                        ItemType = "Service"
                    },
                    message: "تم العثور على معرف الخدمة بنجاح",
                    resultStatus: ResultStatus.Success);
            }

            // If not found in either repository
            return Result<ItemIdResponseDTO>.Fail(
                message: $"لم يتم العثور على عنصر بالمعرف الأساسي {request.BaseItemId}",
                errorType: "NotFound",
                resultStatus: ResultStatus.NotFound);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving item ID for BaseItemId {BaseItemId}", request.BaseItemId);
            return Result<ItemIdResponseDTO>.Fail(
                message: "فشل في جلب معرف العنصر بسبب خطأ في قاعدة البيانات. يرجى المحاولة مرة أخرى لاحقاً.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving item ID for BaseItemId {BaseItemId}: {Message}", request.BaseItemId, ex.Message);
            return Result<ItemIdResponseDTO>.Fail(
                message: "حدث خطأ غير متوقع أثناء جلب معرف العنصر. يرجى المحاولة مرة أخرى لاحقاً.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 