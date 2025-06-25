using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetAllBaseItemsByUserId;
using Catalogs.Application.Queries.GetUserIdByItemId;
using Core.Pagination;
using Core.Result;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaseItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BaseItemController> _logger;

        public BaseItemController(IMediator mediator, ILogger<BaseItemController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("my-items")]
        public async Task<ActionResult<Result<PaginatedResult<BaseItemDTO>>>> GetMyItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.GetId();
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllBaseItemsByUserIdQuery(userId, parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب العناصر",
                    errorType: "GetAllBaseItemsFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<BaseItemDTO>>.Ok(
                data: result.Data,
                message: "تم جلب العناصر بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{baseItemId}/details")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<ItemDetailsDTO>>> GetItemDetailsByBaseItemId(Guid baseItemId)
        {
            var query = new GetItemDetailsByBaseItemIdQuery(baseItemId);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
            {
                if (result.ResultStatus == ResultStatus.NotFound)
                    return NotFound(Result<ItemDetailsDTO>.Fail(
                        message: "العنصر غير موجود",
                        errorType: "ItemNotFound",
                        resultStatus: ResultStatus.NotFound));
                        
                return StatusCode(500, Result<ItemDetailsDTO>.Fail(
                    message: "فشل في جلب تفاصيل العنصر",
                    errorType: "GetItemDetailsFailed",
                    resultStatus: ResultStatus.Failed));
            }
            
            return Ok(Result<ItemDetailsDTO>.Ok(
                data: result.Data,
                message: "تم جلب تفاصيل العنصر بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{itemId}/user-id")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<Guid>>> GetUserIdByItemId(Guid itemId)
        {
            var query = new GetUserIdByItemIdQuery(itemId);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب معرف المستخدم",
                    errorType: "GetUserIdByItemIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<Guid>.Ok(
                data: result.Data,
                message: "تم جلب معرف المستخدم بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 