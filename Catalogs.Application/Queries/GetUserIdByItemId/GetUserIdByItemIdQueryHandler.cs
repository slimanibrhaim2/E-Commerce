using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetUserIdByItemId;

public class GetUserIdByItemIdQueryHandler : IRequestHandler<GetUserIdByItemIdQuery, Result<Guid>>
{
    private readonly IProductRepository _productRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly ILogger<GetUserIdByItemIdQueryHandler> _logger;

    public GetUserIdByItemIdQueryHandler(
        IProductRepository productRepo,
        IServiceRepository serviceRepo,
        ILogger<GetUserIdByItemIdQueryHandler> logger)
    {
        _productRepo = productRepo;
        _serviceRepo = serviceRepo;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(GetUserIdByItemIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ItemId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "معرف العنصر مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // First, try to find as a product
            try
            {
                var product = await _productRepo.GetById(request.ItemId);
                if (product != null)
                {
                    return Result<Guid>.Ok(
                        data: product.UserId,
                        message: $"تم العثور على مالك المنتج بنجاح",
                        resultStatus: ResultStatus.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while checking product repository for item ID {ItemId}", request.ItemId);
            }

            // If not found as product, try to find as a service
            try
            {
                var service = await _serviceRepo.GetById(request.ItemId);
                if (service != null)
                {
                    return Result<Guid>.Ok(
                        data: service.UserId,
                        message: $"تم العثور على مالك الخدمة بنجاح",
                        resultStatus: ResultStatus.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while checking service repository for item ID {ItemId}", request.ItemId);
            }

            // If not found in either repository
            return Result<Guid>.Fail(
                message: $"لم يتم العثور على عنصر بالمعرف {request.ItemId}",
                errorType: "NotFound",
                resultStatus: ResultStatus.NotFound);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving user ID for item with ID {ItemId}", request.ItemId);
            return Result<Guid>.Fail(
                message: "فشل في جلب معرف المستخدم بسبب خطأ في قاعدة البيانات. يرجى المحاولة مرة أخرى لاحقاً.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving user ID for item with ID {ItemId}: {Message}", request.ItemId, ex.Message);
            return Result<Guid>.Fail(
                message: "حدث خطأ غير متوقع أثناء جلب معرف المستخدم. يرجى المحاولة مرة أخرى لاحقاً.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 