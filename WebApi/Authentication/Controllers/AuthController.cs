using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Services;
using WebApi.Authentication.DTOs;
using MediatR;
using Users.Application.Commands.CreateUser;
using Users.Domain.Repositories;
using Core.Result;
using Users.Domain.Entities;
using Microsoft.Extensions.Options;

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
        private readonly JwtService _jwtService;

        public AuthController(
            IOtpService otpService,
            IMediator mediator,
            ILogger<AuthController> logger,
            IUserRepository userRepository,
            JwtService jwtService)
        {
            _otpService = otpService;
            _mediator = mediator;
            _logger = logger;
            _userRepository = userRepository;
            _jwtService = jwtService;
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

                var token = await _otpService.GenerateJwtTokenAsync(user.Id, request.PhoneNumber, user.UserType);
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

        [HttpPost("email-login")]
        public async Task<IActionResult> EmailLogin([FromBody] EmailLoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Result.Fail(
                        message: "البيانات المدخلة غير صحيحة",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                // Check if user exists by email
                var user = await _userRepository.GetByEmail(request.Email);
                if (user == null)
                {
                    return BadRequest(Result.Fail(
                        message: "البريد الإلكتروني أو كلمة المرور غير صحيحة",
                        errorType: "InvalidCredentials",
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

                // Check if user has a password set
                if (string.IsNullOrEmpty(user.Password))
                {
                    return BadRequest(Result.Fail(
                        message: "هذا الحساب لا يدعم تسجيل الدخول بكلمة المرور",
                        errorType: "NoPasswordSet",
                        resultStatus: ResultStatus.ValidationError));
                }

                // Verify password
                if (!PasswordHashingService.VerifyPassword(request.Password, user.Password))
                {
                    return BadRequest(Result.Fail(
                        message: "البريد الإلكتروني أو كلمة المرور غير صحيحة",
                        errorType: "InvalidCredentials",
                        resultStatus: ResultStatus.ValidationError));
                }

                // Generate JWT token with user type
                var token = _jwtService.GenerateToken(user.Id, user.UserType);

                return Ok(Result<string>.Ok(
                    data: token,
                    message: "تم تسجيل الدخول بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email login for {Email}", request.Email);
                return StatusCode(500, Result.Fail(
                    message: "حدث خطأ أثناء تسجيل الدخول",
                    errorType: "EmailLoginError",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Result.Fail(
                        message: "البيانات المدخلة غير صحيحة",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                // Check if user already exists
                var existingUser = await _userRepository.GetByEmail(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(Result.Fail(
                        message: "البريد الإلكتروني مستخدم بالفعل",
                        errorType: "EmailAlreadyExists",
                        resultStatus: ResultStatus.ValidationError));
                }

                existingUser = await _userRepository.GetByPhoneNumber(request.PhoneNumber);
                if (existingUser != null)
                {
                    return BadRequest(Result.Fail(
                        message: "رقم الهاتف مستخدم بالفعل",
                        errorType: "PhoneAlreadyExists",
                        resultStatus: ResultStatus.ValidationError));
                }

                // Hash password
                var hashedPassword = PasswordHashingService.HashPassword(request.Password);

                // Create admin user
                var createUserCommand = new CreateUserCommand(
                    new Users.Application.DTOs.CreateUserDTO
                    {
                        FirstName = request.FirstName,
                        MiddleName = request.MiddleName,
                        LastName = request.LastName,
                        PhoneNumber = request.PhoneNumber,
                        Email = request.Email,
                        Description = request.Description,
                        Password = hashedPassword,
                        UserType = "admin"
                    }
                );

                var result = await _mediator.Send(createUserCommand);
                if (!result.Success)
                {
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تسجيل المشرف",
                        errorType: "RegisterAdminFailed",
                        resultStatus: ResultStatus.Failed));
                }

                return Ok(Result.Ok(
                    message: "تم تسجيل المشرف بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering admin");
                return StatusCode(500, Result.Fail(
                    message: "حدث خطأ أثناء تسجيل المشرف",
                    errorType: "RegisterAdminError",
                    resultStatus: ResultStatus.Failed));
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length == 10 && phoneNumber.StartsWith("09");
        }
    }
}
