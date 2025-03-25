using Eventa_BusinessObject.DTOs.Account;
using Eventa_BusinessObject.DTOs.Email;
using Eventa_BusinessObject.DTOs.Login;
using Eventa_BusinessObject.Entities;
using Eventa_BusinessObject.Enums;
using Eventa_Services.Constant;
using Eventa_Services.Interfaces;
using Eventa_Services.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventa_API.Controllers
{
    [Route("api/accounts")]
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
                ProfilePicture = account.ProfilePicture,
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
            var verifyUrl = $"http://localhost:3000/verify-account?token={verifyToken}";

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
        public async Task<ActionResult<LoginRespone>> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
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
                ProfilePicture = newAccount.ProfilePicture,
                RoleName = newAccount.RoleName
            });
        }
        [HttpGet("getAccount/accountId")]
        public async Task<ActionResult<Account>> GetAccountByAccountId([FromQuery] Guid accountId)
        {
            var account = await _accountService.GetAccountByAccountId(accountId);
            if (account == null)
            {
                return NotFound("Account not found");
            }
            return Ok(account);
        }
        [HttpGet("getAccount/email")]
        public async Task<ActionResult<Account>> GetAccountByEmail([FromQuery] string email)
        {
            var account = await _accountService.GetAccountByEmail(email);
            if (account == null)
            {
                return NotFound("Account not found");
            }
            return Ok(account);
        }
        [HttpPost("update/{accountId}")]
        public async Task<ActionResult<bool>> UpdateAccountById([FromRoute] Guid accountId, [FromForm] UpdateAccountDTO updateAccountDTO)
        {
            var isUpdated = await _accountService.UpdateAccountById(accountId, updateAccountDTO, HttpContext);
            if (!isUpdated)
            {
                return BadRequest("Update failed. Unauthorized or account not found.");
            }
            return Ok("Account updated successfully.");
        }
        [HttpDelete("delete/{accountId}")]
        public async Task<ActionResult<bool>> DeleteAccountById([FromRoute] Guid accountId)
        {
            var isDeleted = await _accountService.DeleteAccountById(accountId, HttpContext);
            if (!isDeleted)
            {
                return NotFound("Account not found or delete failed.");
            }
            return Ok("Account deleted successfully.");
        }
        [HttpPost("calendar")]
        public async Task<IActionResult> AddCalendar([FromBody] CreateCalendarDTO calendar)
        {
            if (calendar == null)
            {
                return BadRequest("Invalid calendar data.");
            }

            var result = await _accountService.AddCalendarToAccount(calendar,HttpContext);
            if (result == "Calendar added successfully")
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { message = result });
        }

        [HttpGet("calendars")]
        public async Task<IActionResult> GetCalendarss()
        {
            var calendars = await _accountService.GetAllCalendarsAsync();
            return Ok(calendars);
        }

        [Authorize] 
        [HttpGet("me")]
        public async Task<ActionResult<AccountResponeGetMe>> GetMe()
        {
            var account = await _accountService.GetCurrentAccount(User);

            if (account == null)
            {
                return Unauthorized("Invalid or expired token.");
            }

            return Ok(new AccountResponeGetMe
            {
                id = account.Id,
                email = account.Email,
                userName = account.Username,
                fullName = account.FullName ?? string.Empty,
                phoneNumber = account.PhoneNumber,
                roleName = account.RoleName,
                profilePicture = account.ProfilePicture,
                address = account.Address,
                bio = account.Bio,
                type = account.Type,
                insDate = account.InsDate,
                updDate = account.UpdDate
            });
        }

        [HttpGet("calendars/accountId")]
        public async Task<IActionResult> GetCalendarsByAccount()
        {
            var calendars = await _accountService.GetCarlendersByAccountID(HttpContext);
            return Ok(calendars);
        }
        [HttpGet("calendar/{publicUrl}")]
        public async Task<IActionResult> GetCalendarByPublicUrl(string publicUrl)
        {
            var calendar = await _accountService.GetCarlendarByPublicUrl(publicUrl,HttpContext);
            if (calendar == null)
            {
                return NotFound(new { message = "Calendar not found" });
            }
            return Ok(calendar);
        }
        [HttpPost("subscribe")]
        public async Task<IActionResult> SubscribeCalendar([FromQuery] string publicUrl)
        {
           
            var success = await _accountService.SubscribeCalendar(publicUrl, HttpContext);
            return success ? Ok("Subscribed successfully") : BadRequest("Failed to subscribe");
        }
        [HttpGet("calendars/not-me")]
        public async Task<IActionResult> GetCalendarsNotMe()
        {
            var calendars = await _accountService.GetListCarlandarNotMe(HttpContext);
            return Ok(calendars);
        }
        [HttpGet("subscribed-calendars")]
        public async Task<ActionResult<List<Calendar>>> GetSubscribedCalendars()
        {
            var httpContext = HttpContext;
            var calendars = await _accountService.GetCalendarsUserSubcribed(httpContext);
            if (calendars == null || calendars.Count == 0)
            {
                return NotFound("No subscribed calendars found.");
            }
            return Ok(calendars);
        }
        [HttpPost("unsubscribe-calendar")]
        public async Task<ActionResult> UnsubscribeCalendar([FromBody] InputCalendar car)
        {
            var httpContext = HttpContext;
            var result = await _accountService.UnsubscribeCalendar(car.PublicUrl, httpContext);
            if (!result)
            {
                return BadRequest("Failed to unsubscribe from the calendar.");
            }
            return Ok("Successfully unsubscribed from the calendar.");
        }


    }
}
