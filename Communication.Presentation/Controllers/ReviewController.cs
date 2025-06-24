using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;
using Communication.Application.Commands.CreateReview;
using Communication.Application.Commands.UpdateReview;
using Communication.Application.Commands.DeleteReview;
using Communication.Application.Queries.GetReviewById;
using Communication.Application.Queries.GetAllReviews;
using Communication.Application.Queries.GetReviewsByBaseItemId;
using Communication.Application.Queries.GetReviewsByUserId;
using Communication.Application.Queries.CheckIfReviewed;
using Core.Pagination;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IMediator mediator, ILogger<ReviewController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Create a new review for a purchased item
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreateReviewDTO dto)
        {
            var userId = User.GetId();
            var command = new CreateReviewCommand(dto, userId);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إضافة المراجعة",
                    errorType: "CreateReviewFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(Result<Guid>.Ok(
                data: result.Data,
                message: "تم إضافة المراجعة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        /// <summary>
        /// Get a specific review by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<ReviewDTO>>> GetById(Guid id)
        {
            var query = new GetReviewByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب المراجعة",
                    errorType: "GetReviewByIdFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(Result<ReviewDTO>.Ok(
                data: result.Data,
                message: "تم جلب المراجعة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        /// <summary>
        /// Get all reviews with pagination (Admin)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<ReviewDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllReviewsQuery(parameters);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب المراجعات",
                    errorType: "GetAllReviewsFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(Result<PaginatedResult<ReviewDTO>>.Ok(
                data: result.Data,
                message: "تم جلب المراجعات بنجاح",
                resultStatus: ResultStatus.Success));
        }

        /// <summary>
        /// Update an existing review (only by the review owner)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] UpdateReviewDTO dto)
        {
            var userId = User.GetId();
            var command = new UpdateReviewCommand(id, dto, userId);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث المراجعة",
                    errorType: "UpdateReviewFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث المراجعة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        /// <summary>
        /// Delete a review (only by the review owner)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var userId = User.GetId();
            var command = new DeleteReviewCommand(id, userId);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف المراجعة",
                    errorType: "DeleteReviewFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف المراجعة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        /// <summary>
        /// Get all reviews for a specific item (Product or Service)
        /// </summary>
        [HttpGet("item/{baseItemId}")]
        public async Task<ActionResult<Result<PaginatedResult<ReviewDTO>>>> GetByBaseItemId(Guid baseItemId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetReviewsByBaseItemIdQuery(baseItemId, parameters);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب مراجعات العنصر",
                    errorType: "GetReviewsByBaseItemIdFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(Result<PaginatedResult<ReviewDTO>>.Ok(
                data: result.Data,
                message: "تم جلب مراجعات العنصر بنجاح",
                resultStatus: ResultStatus.Success));
        }

        /// <summary>
        /// Get all reviews by the current user
        /// </summary>
        [HttpGet("my-reviews")]
        public async Task<ActionResult<Result<PaginatedResult<ReviewDTO>>>> GetMyReviews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.GetId();
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetReviewsByUserIdQuery(userId, parameters);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب مراجعاتك",
                    errorType: "GetMyReviewsFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(Result<PaginatedResult<ReviewDTO>>.Ok(
                data: result.Data,
                message: "تم جلب مراجعاتك بنجاح",
                resultStatus: ResultStatus.Success));
        }

        /// <summary>
        /// Check if the current user has already reviewed a specific item
        /// </summary>
        [HttpGet("check/{itemId}")]
        public async Task<ActionResult<Result<bool>>> CheckIfReviewed(Guid itemId)
        {
            var userId = User.GetId();
            var query = new CheckIfReviewedQuery(itemId, userId);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في فحص حالة المراجعة",
                    errorType: "CheckIfReviewedFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم فحص حالة المراجعة بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 