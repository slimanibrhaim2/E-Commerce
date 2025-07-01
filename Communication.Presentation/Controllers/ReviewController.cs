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
using Communication.Application.Queries.GetReviewsByProviderId;
using Communication.Application.Queries.GetReviewsByOrderId;
using Communication.Application.Queries.GetReviewsByUserId;
using Communication.Application.Queries.CheckIfReviewed;
using Core.Pagination;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using System;
using System.Threading.Tasks;

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
        /// Get a review by its ID
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
                    
            return Ok(result);
        }

        /// <summary>
        /// Get all reviews with pagination
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
                    
            return Ok(result);
        }

        /// <summary>
        /// Get all reviews for a specific provider
        /// </summary>
        [HttpGet("provider/{providerId}")]
        public async Task<ActionResult<Result<PaginatedResult<ReviewDTO>>>> GetByProviderId(Guid providerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetReviewsByProviderIdQuery(providerId, parameters);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب مراجعات المزود",
                    errorType: "GetReviewsByProviderIdFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(result);
        }

        /// <summary>
        /// Get all reviews for a specific order
        /// </summary>
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<Result<PaginatedResult<ReviewDTO>>>> GetByOrderId(Guid orderId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetReviewsByOrderIdQuery(orderId, parameters);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب مراجعات الطلب",
                    errorType: "GetReviewsByOrderIdFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(result);
        }

        /// <summary>
        /// Check if the current user has already reviewed a specific order
        /// </summary>
        [HttpGet("check/{orderId}")]
        public async Task<ActionResult<Result<bool>>> CheckIfReviewed(Guid orderId)
        {
            var userId = User.GetId();
            var query = new CheckIfReviewedQuery(orderId, userId);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في فحص حالة المراجعة",
                    errorType: "CheckIfReviewedFailed",
                    resultStatus: ResultStatus.Failed));
                    
            return Ok(result);
        }

        /// <summary>
        /// Create a new review
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Result>> Create([FromBody] CreateReviewDTO dto)
        {
            var userId = User.GetId();
            var command = new CreateReviewCommand(dto, userId);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, result);
                    
            return Ok(result);
        }

        /// <summary>
        /// Update an existing review
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Result>> Update(Guid id, [FromBody] UpdateReviewDTO dto)
        {
            var userId = User.GetId();
            var command = new UpdateReviewCommand(id, dto, userId);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, result);
                    
            return Ok(result);
        }

        /// <summary>
        /// Delete a review
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Result>> Delete(Guid id)
        {
            var userId = User.GetId();
            var command = new DeleteReviewCommand(id, userId);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, result);
                    
            return Ok(result);
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
    }
} 