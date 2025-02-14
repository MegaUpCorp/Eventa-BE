using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.DTOs.Email;
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
        private readonly IEmailService _emailService;
        private readonly IVerificationTokenService _verificationTokenService;

        public AccountController(IAccountService accountService,
                                 IAuthService authService,
                                 IEmailService emailService,
                                 IVerificationTokenService verificationTokenService)
        {
            _accountService = accountService;
            _authService = authService;
            _emailService = emailService;
            _verificationTokenService = verificationTokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginRespone>> Login([FromBody] LoginRequest loginRequest)
        {
            var account = await _authService.Authenticate(loginRequest.Email, loginRequest.Password);
            if (account == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = await _authService.GenerateJwtToken(account);
            return Ok(new LoginRespone
            {
                Token = token,
                Phone = account.PhoneNumber,
                RoleName = account.RoleName
            });
        }

        [HttpPost("register/request")]
        public async Task<ActionResult> RequestRegistration([FromBody] RequestRegisterDTO request)
        {
            if (await _accountService.IsEmailExists(request.Email))
            {
                return BadRequest("Email already in use.");
            }

            var verifyToken = _verificationTokenService.GenerateToken(request.Email);

            // Thêm redirectUrl vào query string
            var verifyUrl = $"https://localhost:7298/api/account/verify?token={verifyToken}&redirectUrl=https://localhost:7298/swagger/index.html";

            await _emailService.SendVerificationEmail(request.Email, verifyUrl);

            return Ok("Verification email sent.");
        }

        [HttpPost("register/verify")]
        public async Task<ActionResult<bool>> VerifyEmailToken([FromBody] VerifyEmailTokenRequest request)
        {
            bool isValid = _verificationTokenService.ValidateToken(request.Token, out _);
            return Ok(isValid);
        }

        [HttpPost("register/complete")]
        public async Task<ActionResult<LoginRespone>> CompleteRegistration([FromForm] CompleteRegistrationRequest request)
        {
            // Xác thực token và lấy email từ token
            if (!_verificationTokenService.ValidateToken(request.Token, out string? email) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid or expired token.");
            }

            // Tạo tài khoản mới với email lấy từ token
            var newAccount = await _accountService.Register(email, request);
            var accessToken = await _authService.GenerateJwtToken(newAccount);

            return Ok(new LoginRespone
            {
                Token = accessToken,
                Phone = newAccount.PhoneNumber,
                RoleName = newAccount.RoleName
            });
        }
    }
}
