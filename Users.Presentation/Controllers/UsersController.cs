// Users.API/Controllers/UsersController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.DTOs;
using Users.Application.Commands.CreateUser;
using Users.Application.Commands.DeleteUser;
using Users.Application.Queries.GetAllUsers;
using Users.Application.Queries.GetUserById;
using Core.Result;
using Users.Application.Command.UpdateUser;
using Core.Pagination;
using Users.Application.Queries.GetUsersByName;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Authentication;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileService _fileService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, IFileService fileService, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] CreateUserDTO dto, IFormFile? profileImage)
        {
            try
            {
                string? imageUrl = null;
                if (profileImage != null)
                {
                    // Validate image
                    var validationResult = _fileService.ValidateFile(
                        profileImage,
                        new[] { "image/jpeg", "image/png", "image/gif" },
                        5 * 1024 * 1024 // 5MB
                    );

                    if (!validationResult.Success)
                        return BadRequest(validationResult);

                    // Save image
                    var fileResult = await _fileService.SaveFileAsync(profileImage, "images/profiles");
                    if (!fileResult.Success)
                        return StatusCode(500, fileResult);

                    imageUrl = fileResult.Data;
                }

                // Set the image URL in the DTO
                dto.ProfilePhoto = imageUrl;

                var result = await _mediator.Send(new CreateUserCommand(dto));
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في إنشاء المستخدم",
                        errorType: "CreateUserFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(Result<Guid>.Ok(
                    data: result.Data,
                    message: "تم إنشاء المستخدم بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Result.Fail(
                    message: $"فشل في إنشاء المستخدم: {ex.Message}",
                    errorType: "CreateUserFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
                var query = new GetAllUsersQuery(pagination);
                var result = await _mediator.Send(query);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في جلب المستخدمين",
                        errorType: "GetAllFailed",
                        resultStatus: ResultStatus.Failed));

                // No need to load image data anymore, just return the URLs
                return Ok(Result<PaginatedResult<UserDTO>>.Ok(
                    data: result.Data,
                    message: "تم جلب المستخدمين بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users");
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب المستخدمين",
                    errorType: "GetAllFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.GetId();
                
                var query = new GetUserByIdQuery(userId);
                var result = await _mediator.Send(query);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في جلب بيانات المستخدم",
                        errorType: "GetByIdFailed",
                        resultStatus: ResultStatus.Failed));

                // No need to load image data anymore, just return the URL
                return Ok(Result<UserDTO>.Ok(
                    data: result.Data,
                    message: "تم جلب بيانات المستخدم بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user data for ID: {UserId}", User.GetId());
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب بيانات المستخدم",
                    errorType: "GetByIdFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromForm] CreateUserDTO dto, IFormFile? profileImage)
        {
            try
            {
                var userId = User.GetId();
                _logger.LogInformation("Updating user {UserId} with image: {HasImage}", userId, profileImage != null);

                // Get current user to handle profile photo update
                var currentUser = await _mediator.Send(new GetUserByIdQuery(userId));
                string? oldProfilePhoto = currentUser.Success ? currentUser.Data.ProfilePhoto : null;

                if (profileImage != null)
                {
                    // Validate image
                    var validationResult = _fileService.ValidateFile(
                        profileImage,
                        new[] { "image/jpeg", "image/png", "image/gif" },
                        5 * 1024 * 1024 // 5MB
                    );

                    if (!validationResult.Success)
                    {
                        _logger.LogWarning("File validation failed for user {UserId}: {Error}", userId, validationResult.Message);
                        return BadRequest(validationResult);
                    }

                    // Save new image
                    var fileResult = await _fileService.SaveFileAsync(profileImage, "images/profiles");
                    if (!fileResult.Success)
                    {
                        _logger.LogError("File save failed for user {UserId}: {Error}", userId, fileResult.Message);
                        return StatusCode(500, fileResult);
                    }

                    _logger.LogInformation("File saved successfully for user {UserId}: {Path}", userId, fileResult.Data);
                    dto.ProfilePhoto = fileResult.Data;

                    // Delete old profile photo if it exists
                    if (!string.IsNullOrEmpty(oldProfilePhoto))
                    {
                        var deleteResult = _fileService.DeleteFile(oldProfilePhoto);
                        if (!deleteResult.Success)
                        {
                            _logger.LogWarning("Failed to delete old profile photo for user {UserId}: {Error}", 
                                userId, deleteResult.Message);
                        }
                    }
                }
                else
                {
                    // If no new image is provided, keep the existing one
                    dto.ProfilePhoto = oldProfilePhoto;
                }

                var result = await _mediator.Send(new UpdateUserCommand(userId, dto));
                if (!result.Success)
                {
                    _logger.LogError("Update failed for user {UserId}: {Error}", userId, result.Message);
                    return StatusCode(500, Result.Fail(
                        message: result.Message,
                        errorType: result.ErrorType,
                        resultStatus: result.ResultStatus));
                }

                return Ok(Result.Ok(
                    message: "تم تحديث المستخدم بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating user {UserId}", User.GetId());
                return StatusCode(500, Result.Fail(
                    message: $"فشل في تحديث المستخدم: {ex.Message}",
                    errorType: "UpdateUserFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            try
            {
                var userId = User.GetId();

                // Get the current user to find their profile photo
                var currentUser = await _mediator.Send(new GetUserByIdQuery(userId));
                if (currentUser.Success && !string.IsNullOrEmpty(currentUser.Data.ProfilePhoto))
                {
                    // Delete the profile photo
                    var deleteResult = _fileService.DeleteFile(currentUser.Data.ProfilePhoto);
                    if (!deleteResult.Success)
                    {
                        _logger.LogWarning("Failed to delete profile photo for user {UserId}: {Error}", 
                            userId, deleteResult.Message);
                    }
                }

                var result = await _mediator.Send(new DeleteUserCommand(userId));
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في حذف المستخدم",
                        errorType: "DeleteUserFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(Result.Ok(
                    message: "تم حذف المستخدم بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", User.GetId());
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف المستخدم",
                    errorType: "DeleteUserFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
                var query = new GetUsersByNameQuery(name, parameters);
                var result = await _mediator.Send(query);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في البحث عن المستخدمين",
                        errorType: "SearchByNameFailed",
                        resultStatus: ResultStatus.Failed));


                return Ok(Result<PaginatedResult<UserDTO>>.Ok(
                    data: result.Data,
                    message: "تم جلب المستخدمين بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users by name: {Name}", name);
                return StatusCode(500, Result.Fail(
                    message: "فشل في البحث عن المستخدمين",
                    errorType: "SearchByNameFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
}
