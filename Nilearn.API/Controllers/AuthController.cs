using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nilearn.Application.Features.Auth.Login.Commands;
using Nilearn.Application.Features.Auth.Login.DTOs;
using Nilearn.Application.Features.Auth.RefreshToken.Command;
using Nilearn.Application.Features.Auth.Register.Student.Commands;
using Nilearn.Application.Features.Auth.Register.Student.DTOs;

namespace Nilearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _mediator.Send(new LoginCommand(loginRequestDto));
            if (!result.Success)
            {
                _logger.LogWarning("Login attempt failed for email: {Email}. Errors: {Errors}", loginRequestDto.Email, string.Join(", ", result.Errors ?? new List<string>()));
                return Ok(result);
            }
            SetRefreshTokenCookie(result.Data.RefreshToken, result.Data.ExpiresAt);
            //SetAccessTokenCookie(result.Data.AccessToken);

            _logger.LogInformation("Login successful for email: {Email}", loginRequestDto.Email);
            return Ok(result);
        }
        [HttpPost]
        [Route("register-student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequestDto registerStudentRequestDto)
        {
            var result = await _mediator.Send(new RegisterStudentCommand(registerStudentRequestDto));
            if (!result.Success)
            {
                _logger.LogWarning("Student registration failed for email: {Email}. Errors: {Errors}", registerStudentRequestDto.Email, string.Join(", ", result.Errors ?? new List<string>()));
                return BadRequest(result);
            }
            _logger.LogInformation("Student registration successful for email: {Email}", registerStudentRequestDto.Email);
            return Ok(result);
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                _logger.LogWarning("Refresh token not found in cookies.");
                return BadRequest(new { Message = "Refresh token is missing." });
            }
            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));
            if (!result.Success)
            {
                _logger.LogWarning("Refresh token attempt failed. Errors: {Errors}", string.Join(", ", result.Errors ?? new List<string>()));
                return BadRequest(result);
            }
            SetRefreshTokenCookie(result.Data.RefreshToken, result.Data.ExpiresAt);
            //SetAccessTokenCookie(result.Data.AccessToken);
            _logger.LogInformation("Refresh token successful.");
            return Ok(result);
        }
        #region Helpers
        private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAt
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
        //private void SetAccessTokenCookie(string accessToken)
        //{
        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.Strict,
        //        Expires = DateTime.UtcNow.AddMinutes(15)

        //    };
        //    Response.Cookies.Append("accessToken", accessToken, cookieOptions);
        //} 
        #endregion
    }
   
}