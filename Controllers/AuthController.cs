using KPMGTask.Dtos;
using KPMGTask.Services.AuthenticationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KPMGTask.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService
            )
        {
            _authService = authService;


        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            //  SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            // return Ok(new {token=result.Token,expireOn=result.ExpiresOn});

            return Ok(result);
        }
      


        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestDto model)
        {
            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            }

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }

            return Ok(model);
        }


        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result);
            }
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);

        }


        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }
            var result = await _authService.RevokeTokenAsync(token);
            if (!result)
                return BadRequest("Token is invalid");

            return Ok();
        }


    


        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDTO request)
        {
            var result = await _authService.ForgotPassword(request);

            if (result.Status)
            {
                return Ok();
            }

            return BadRequest(result.Message);
        }

        [AllowAnonymous]
        [HttpPost("SetNewPasswordForForgotPasword")]
        public async Task<IActionResult> SetNewPasswordForForgotPasword(SetNewPasswordForForgotPasswordDTO request)
        {
            var result = await _authService.SetNewPassword(request, true);

            if (result)
            {
                return Ok();
            }

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("SetPasswordForNewUser")]
        public async Task<IActionResult> SetPasswordForNewUser(SetNewPasswordForForgotPasswordDTO request)
        {
            var result = await _authService.SetNewPassword(request, false);

            if (result)
            {
                return Ok();
            }

            return BadRequest(result);
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO request)
        {
            var result = await _authService.ResetPassword(request);

            if (result.Status)
            {
                return Ok();
            }

            return BadRequest(result.Message);
        }



        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = true, // for http false and true for https
                SameSite = SameSiteMode.None,
                Domain = "localhost", //using https://localhost:44340/ here doesn't work
                Expires = expires.ToLocalTime()
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }





    }
}
