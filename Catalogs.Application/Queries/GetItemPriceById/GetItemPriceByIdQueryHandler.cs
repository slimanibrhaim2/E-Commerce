using Core.Result;
using MediatR;
using Shared.Contracts.Queries;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetItemPriceById;

public class GetItemPriceByIdQueryHandler : IRequestHandler<GetItemPriceByIdQuery, Result<GetItemPriceByIdResponse>>
{
    private readonly IBaseItemRepository _baseItemRepository;
    private readonly ILogger<GetItemPriceByIdQueryHandler> _logger;

    public GetItemPriceByIdQueryHandler(
        IBaseItemRepository baseItemRepository,
        ILogger<GetItemPriceByIdQueryHandler> logger)
    {
        _baseItemRepository = baseItemRepository;
        _logger = logger;
    }

    public async Task<Result<GetItemPriceByIdResponse>> Handle(GetItemPriceByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _baseItemRepository.GetByIdAsync(request.ItemId);
            
            if (item == null)
                return Result<GetItemPriceByIdResponse>.Fail(
                    message: "العنصر غير موجود",
                    errorType: "ItemNotFound",
                    resultStatus: ResultStatus.NotFound);

            var response = new GetItemPriceByIdResponse(
                Price: item.Price
            );

            return Result<GetItemPriceByIdResponse>.Ok(
                data: response,
                message: "تم جلب سعر العنصر بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting price for item {ItemId}", request.ItemId);
            return Result<GetItemPriceByIdResponse>.Fail(
                message: "فشل في جلب سعر العنصر",
                errorType: "GetItemPriceFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 