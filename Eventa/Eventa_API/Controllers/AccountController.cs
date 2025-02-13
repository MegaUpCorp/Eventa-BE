using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.DTOs.Login;
using Eventa_Services.Constant;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;
        public AccountController(IAccountService accountService, IAuthService authService)
        {
            _accountService = accountService;
            _authService = authService;
        }
        [HttpPost(APIEndPointConstant.Authentication.Login)]
        [ProducesResponseType(typeof(LoginRespone), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(UnauthorizedObjectResult))]
        public async Task<ActionResult<string>> Login([FromForm] LoginRequest loginRequest)
        {
            var account = await _authService.Authenticate(loginRequest.Email, loginRequest.Password);
            if (account == null)
            {
                return Unauthorized();
            }
            var token = await _authService.GenerateJwtToken(account);
            var loginRespone = new LoginRespone
            {
                Token = token,
                // Thêm UserName

                Phone = account.PhoneNumber,
                RoleName = account.RoleName
            };
            return Ok(loginRespone);
        }
        [HttpPost(APIEndPointConstant.Account.Register)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public async Task<ActionResult<bool>> Register([FromForm] CreateAccountRequest createAccountRequest)
        {
            var result = await _accountService.Register(createAccountRequest);
            return Ok(result);
        }
    }
}
