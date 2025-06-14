using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Services;
using WebApi.Authentication.DTOs;
using MediatR;
using Users.Application.Commands.CreateUser;
using Users.Domain.Repositories;

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
                var result = await _otpService.SendOtpAsync(request.PhoneNumber);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to send OTP" });
                }

                return Ok(new { message = "OTP sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {PhoneNumber}", request.PhoneNumber);
                return StatusCode(500, new { message = "An error occurred while sending OTP" });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                var user = await _userRepository.GetByPhoneNumber(request.PhoneNumber);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                var isValid = await _otpService.VerifyOtpAsync(request.PhoneNumber, request.Otp);
                if (!isValid)
                {
                    return BadRequest(new { message = "Invalid OTP" });
                }

                var token = await _otpService.GenerateJwtTokenAsync(user.Id, request.PhoneNumber);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {PhoneNumber}", request.PhoneNumber);
                return StatusCode(500, new { message = "An error occurred while verifying OTP" });
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
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, new { message = "An error occurred while registering user" });
            }
        }
    }
}
