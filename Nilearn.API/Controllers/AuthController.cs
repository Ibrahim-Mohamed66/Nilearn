using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nilearn.Application.Features.Auth.EmailVerification.ConfirmEmailVerification.Commands;
using Nilearn.Application.Features.Auth.EmailVerification.ConfirmEmailVerification.DTOs;
using Nilearn.Application.Features.Auth.Login.Commands;
using Nilearn.Application.Features.Auth.Login.DTOs;
using Nilearn.Application.Features.Auth.Logout.Commands;
using Nilearn.Application.Features.Auth.RefreshToken.Command;
using Nilearn.Application.Features.Auth.Register.Instructor.Commands;
using Nilearn.Application.Features.Auth.Register.Instructor.DTOs;
using Nilearn.Application.Features.Auth.Register.Student.Commands;
using Nilearn.Application.Features.Auth.Register.Student.DTOs;

namespace Nilearn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LoginCommand(loginRequestDto), cancellationToken);

            if (!result.Success)
                return Unauthorized(result);

            SetRefreshTokenCookie(result.Data.RefreshToken, result.Data.ExpiresAt);
            //SetAccessTokenCookie(result.Data.AccessToken);
            return Ok(result);
        }
        [HttpPost("register/instructor")]
        public async Task<IActionResult> RegisterInstructor([FromBody] InstructorRequestDto instructorRequestDto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RegisterInstructorCommand(instructorRequestDto), cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequestDto registerStudentRequestDto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RegisterStudentCommand(registerStudentRequestDto), cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return BadRequest(new { Message = "Refresh token is missing." });

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);

            if (!result.Success)
                return Unauthorized(result);

            SetRefreshTokenCookie(result.Data.RefreshToken, result.Data.ExpiresAt);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return BadRequest(new { Message = "Refresh token is missing." });

            var result = await _mediator.Send(new LogoutCommand(refreshToken), cancellationToken);

            if (!result.Success)
                return BadRequest(result);

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            return Ok(result);
        }

        [HttpGet("email/confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(request), cancellationToken);

            if (!result.Success)
                return BadRequest(result);

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
                Expires = expiresAt,
                Path = "/"
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
        // TO DO
        //private void SetAccessTokenCookie(string  accessToken)
        //{
        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.Strict,
        //        Expires = DateTime.UtcNow.AddMinutes(15)
        //        Path = "/"
        //    };
        //    Response.Cookies.Append("accessToken", accessToken, cookieOptions);
        //} 
        #endregion
    }
   
}