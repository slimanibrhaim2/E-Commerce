using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;
using Communication.Application.Commands.CreateMessage;
using Communication.Application.Commands.UpdateMessage;
using Communication.Application.Commands.DeleteMessage;
using Communication.Application.Queries.GetMessageById;
using Communication.Application.Queries.GetAllMessages;
using Core.Pagination;
using Communication.Application.Commands.AddMessageAggregate;
using Communication.Application.Commands.UpdateMessageAggregate;
using Communication.Application.Commands.DeleteMessageAggregate;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MessageController> _logger;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MessageController(
            IMediator mediator, 
            ILogger<MessageController> logger,
            IFileService fileService,
            IWebHostEnvironment webHostEnvironment)
        {
            _mediator = mediator;
            _logger = logger;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userId;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateMessageDTO dto)
        {
            try
            {
                dto.SenderId = GetCurrentUserId(); // Always set sender from token
                var command = new CreateMessageCommand(dto);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في إنشاء الرسالة",
                        errorType: "CreateMessageFailed",
                        resultStatus: ResultStatus.Failed));
                return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                    data: result.Data,
                    message: "تم إنشاء الرسالة بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, Result.Fail(
                    message: "غير مصرح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError));
            }
        }

        [HttpPost("aggregate")]
        public async Task<ActionResult<Result<Guid>>> AddMessageAggregate([FromForm] CreateMessageRequest request)
        {
            try
            {
                var attachmentUrls = new List<AttachmentDTO>();

                // Handle file uploads if any
                if (request.Attachments != null && request.Attachments.Any())
                {
                    foreach (var file in request.Attachments)
                    {
                        // Validate file
                        var validationResult = _fileService.ValidateFile(
                            file,
                            new[] { "image/jpeg", "image/png", "image/gif", "application/pdf", "application/msword", 
                                   "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                            10 * 1024 * 1024 // 10MB limit
                        );

                        if (!validationResult.Success)
                        {
                            _logger.LogWarning("فشل التحقق من صحة الملف: {Error}", validationResult.Message);
                            return BadRequest(validationResult);
                        }

                        // Save file
                        var saveResult = await _fileService.SaveFileAsync(file, "media/messages");
                        if (!saveResult.Success)
                        {
                            return StatusCode(500, Result.Fail(
                                message: "فشل في حفظ الملف",
                                errorType: "SaveFileFailed",
                                resultStatus: ResultStatus.Failed));
                        }

                        // Add attachment URL to the list
                        attachmentUrls.Add(new AttachmentDTO 
                        { 
                            AttachmentUrl = saveResult.Data,
                            AttachmentTypeId = Guid.NewGuid() // You might want to handle attachment types properly
                        });
                    }
                }

                // Create the DTO for the command
                var dto = new AddMessageAggregateDTO
                {
                    ConversationId = request.ConversationId,
                    Content = request.Content,
                    SenderId = GetCurrentUserId(),
                    ReceiverId = request.ReceiverId,
                    Attachments = attachmentUrls
                };

                var command = new AddMessageAggregateCommand(dto);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "Failed to create message",
                        errorType: "CreateMessageFailed",
                        resultStatus: ResultStatus.Failed));

                return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                    data: result.Data,
                    message: "Message created successfully",
                    resultStatus: ResultStatus.Success));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, Result.Fail(
                    message: "Unauthorized",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating message");
                return StatusCode(500, Result.Fail(
                    message: $"Failed to create message: {ex.Message}",
                    errorType: "CreateMessageFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<MessageDTO>>> GetById(Guid id)
        {
            var query = new GetMessageByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الرسالة",
                    errorType: "GetMessageFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<MessageDTO>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllMessagesQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الرسائل",
                    errorType: "GetMessagesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateMessageDTO dto)
        {
            try
            {
                dto.SenderId = GetCurrentUserId(); // Always set sender from token
                var command = new UpdateMessageCommand(id, dto);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تحديث الرسالة",
                        errorType: "UpdateMessageFailed",
                        resultStatus: ResultStatus.Failed));
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, Result.Fail(
                    message: "غير مصرح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError));
            }
        }

        [HttpPut("aggregate/{id}")]
        public async Task<ActionResult<Result<Guid>>> UpdateMessageAggregate(Guid id, AddMessageAggregateDTO dto)
        {
            try
            {
                dto.SenderId = GetCurrentUserId(); // Always set sender from token
                var command = new UpdateMessageAggregateCommand(id, dto);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تحديث الرسالة",
                        errorType: "UpdateMessageFailed",
                        resultStatus: ResultStatus.Failed));
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, Result.Fail(
                    message: "غير مصرح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteMessageCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الرسالة",
                    errorType: "DeleteMessageFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(result);
        }

        [HttpDelete("aggregate/{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteMessageAggregate(Guid id)
        {
            var command = new DeleteMessageAggregateCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الرسالة",
                    errorType: "DeleteMessageFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(result);
        }

        [HttpGet("attachment/{*filePath}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMessageAttachment(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(Result.Fail(
                        message: "File path is required",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                // Prevent directory traversal
                filePath = filePath.Replace("\\", "/").TrimStart('/');
                if (filePath.Contains(".."))
                {
                    return BadRequest(Result.Fail(
                        message: "Invalid file path",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                var result = await _fileService.GetFileAsync(filePath);
                if (!result.Success)
                {
                    return NotFound(Result.Fail(
                        message: "File not found",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.NotFound));
                }

                // Determine content type
                var contentType = "application/octet-stream";
                var extension = Path.GetExtension(filePath).ToLower();
                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    case ".doc":
                        contentType = "application/msword";
                        break;
                    case ".docx":
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                }

                // Add cache headers
                Response.Headers.Add("Cache-Control", "public, max-age=604800"); // Cache for 1 week
                Response.Headers.Add("Expires", DateTime.UtcNow.AddDays(7).ToString("R"));

                return File(result.Data, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving message attachment");
                return StatusCode(500, Result.Fail(
                    message: "Failed to serve file",
                    errorType: "GetFileFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
} 