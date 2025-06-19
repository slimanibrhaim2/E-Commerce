using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Services;
using WebApi.Authentication.DTOs;
using MediatR;
using Users.Application.Commands.CreateUser;
using Users.Domain.Repositories;
using Core.Result;

namespace WebApi.Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IOtpService _otpService;
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        private readonly IUserRepository _userRepository;

        public AuthController(
            IOtpService otpService,
            IMediator mediator,
            ILogger<AuthController> logger,
            IUserRepository userRepository)
        {
            _otpService = otpService;
            _mediator = mediator;
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validate phone number format
                if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                {
                    return BadRequest(Result.Fail(
                        message: "يرجى إدخال رقم الهاتف",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                if (!IsValidPhoneNumber(request.PhoneNumber))
                {
                    return BadRequest(Result.Fail(
                        message: "يرجى إدخال رقم هاتف صحيح (يبدأ بـ 09 ويتكون من 10 أرقام)",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                // Check if user exists
                var user = await _userRepository.GetByPhoneNumber(request.PhoneNumber);
                if (user == null)
                {
                    return BadRequest(Result.Fail(
                        message: "رقم الهاتف غير مسجل في النظام. يرجى التسجيل أولاً",
                        errorType: "UserNotFound",
                        resultStatus: ResultStatus.NotFound));
                }

                // Check if user is deleted
                if (user.DeletedAt != null)
                {
                    return BadRequest(Result.Fail(
                        message: "الحساب محذوف. يرجى التواصل مع الدعم الفني",
                        errorType: "UserDeleted",
                        resultStatus: ResultStatus.ValidationError));
                }

                var result = await _otpService.SendOtpAsync(request.PhoneNumber);
                if (!result)
                {
                    return StatusCode(500, Result.Fail(
                        message: "فشل في إرسال رمز التحقق",
                        errorType: "SendOtpFailed",
                        resultStatus: ResultStatus.Failed));
                }

                return Ok(Result.Ok(
                    message: "تم إرسال رمز التحقق بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {PhoneNumber}", request.PhoneNumber);
                return StatusCode(500, Result.Fail(
                    message: "حدث خطأ أثناء إرسال رمز التحقق",
                    errorType: "SendOtpError",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                // Validate phone number format
                if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                {
                    return BadRequest(Result.Fail(
                        message: "يرجى إدخال رقم الهاتف",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                if (!IsValidPhoneNumber(request.PhoneNumber))
                {
                    return BadRequest(Result.Fail(
                        message: "يرجى إدخال رقم هاتف صحيح (يبدأ بـ 09 ويتكون من 10 أرقام)",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(Result.Fail(
                        message: "صيغة رمز التحقق غير صحيحة",
                        errorType: "InvalidOtpFormat",
                        resultStatus: ResultStatus.Failed));
                }

                var user = await _userRepository.GetByPhoneNumber(request.PhoneNumber);
                if (user == null)
                {
                    return BadRequest(Result.Fail(
                        message: "المستخدم غير موجود",
                        errorType: "UserNotFound",
                        resultStatus: ResultStatus.Failed));
                }

                // Check if user is deleted
                if (user.DeletedAt != null)
                {
                    return BadRequest(Result.Fail(
                        message: "الحساب محذوف. يرجى التواصل مع الدعم الفني",
                        errorType: "UserDeleted",
                        resultStatus: ResultStatus.ValidationError));
                }

                var isValid = await _otpService.VerifyOtpAsync(request.PhoneNumber, request.Otp);
                if (!isValid)
                {
                    return BadRequest(Result.Fail(
                        message: "رمز التحقق غير صحيح",
                        errorType: "InvalidOtp",
                        resultStatus: ResultStatus.Failed));
                }

                var token = await _otpService.GenerateJwtTokenAsync(user.Id, request.PhoneNumber);
                return Ok(Result<string>.Ok(
                    data: token,
                    message: "تم التحقق من رمز التحقق بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {PhoneNumber}", request.PhoneNumber);
                return StatusCode(500, Result.Fail(
                    message: "حدث خطأ أثناء التحقق من رمز التحقق",
                    errorType: "VerifyOtpError",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (!result.Success)
                {
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تسجيل المستخدم",
                        errorType: "RegisterUserFailed",
                        resultStatus: ResultStatus.Failed));
                }

                // Send OTP after successful registration
                var otpResult = await _otpService.SendOtpAsync(command.userDTO.PhoneNumber);
                if (!otpResult)
                {
                    _logger.LogWarning("Failed to send OTP after registration for {PhoneNumber}", command.userDTO.PhoneNumber);
                    return Ok(Result.Ok(
                        message: "تم تسجيل المستخدم بنجاح، ولكن فشل إرسال رمز التحقق. يرجى محاولة تسجيل الدخول",
                        resultStatus: ResultStatus.Success));
                }

                return Ok(Result.Ok(
                    message: "تم تسجيل المستخدم بنجاح. يرجى التحقق من رقم هاتفك باستخدام رمز التحقق المرسل",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, Result.Fail(
                    message: "حدث خطأ أثناء تسجيل المستخدم",
                    errorType: "RegisterUserError",
                    resultStatus: ResultStatus.Failed));
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length == 10 && phoneNumber.StartsWith("09");
        }
    }
}
