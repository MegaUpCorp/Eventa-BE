using Eventa_BusinessObject.DTOs.Login;
using Eventa_BusinessObject.Validations;
using Eventa_Services.Implements;
using Eventa_Services.Interfaces;
using Eventa_Services.Share;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Eventa_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("signin-google")]
        public IActionResult SignInWithGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = "/api/auth/google-callback" };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpPost("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromBody] dynamic data)
        {
            try
            {
                if (data?.token == null)
                {
                    Console.WriteLine("Missing token in request.");
                    return BadRequest(new { message = "Token is required." });
                }

                string token = data.token;
                Console.WriteLine($"Received Token: {token}");

                var user = await _authService.GoogleCallback(token);
                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GoogleCallback: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server ", error = ex.Message });
            }
        }
        [HttpPost("login-google")]
        [ProducesResponseType(200, Type = typeof(Result<object>))]
        [ProducesResponseType(400, Type = typeof(Result<object>))]

        public async Task<IActionResult> LoginGoogle([FromBody] GoogleRequest req)
        {
            var validator = new GoogleLoginRequestValidator();
            var validatorResult = validator.Validate(req);
            if (!validatorResult.IsValid)
            {
                return BadRequest(new Result<object>
                {
                    Error = 1,
                    Message = "Missing or invalid value!",
                    Data = validatorResult.Errors.Select(x => x.ErrorMessage),
                });
            }
            var result = await _authService.GoogleCallback(req.AccessToken);

            if (result?.Error == 1)
            {
                return Ok(result);
            }

            LoginResponse<LoginData> response = result?.Data as LoginResponse<LoginData>;

            if (response == null)
            {
                return Ok(new Result<object>
                {
                    Error = 1,
                    Message = "Please log in again!",
                    Data = null
                });
            }

            Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.Strict,
            });

            return Ok(new Result<object>
            {
                Error = 0,
                Message = "Login successfully",
                Data = new
                {
                    AccessToken = new
                    {
                        Token_type = "Bearer",
                        Token = response.AccessToken,
                    },
                    User = response.Data
                }
            });

        }
    }
}
