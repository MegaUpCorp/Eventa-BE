using Eventa_BusinessObject;
using Eventa_BusinessObject.Entities;
using Eventa_Repositories.Interfaces;
using Eventa_Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using static Eventa_Services.Implements.GoogleOauthService;
using Eventa_BusinessObject.Enums;
using Eventa_Repositories.Implements;
using Eventa_BusinessObject.DTOs.Login;
using Eventa_Services.Share;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Eventa_Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGoogleOauthService _googleOauthService;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;
        private readonly AccessTokenGenerator _accessTokenGenerator;


        public AuthService(IAccountRepository accountRepository, IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor, IGoogleOauthService googleOauthService, RefreshTokenGenerator refreshTokenGenerator, AccessTokenGenerator accessTokenGenerator)
        {
            _accountRepository = accountRepository;
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _googleOauthService = googleOauthService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _accessTokenGenerator = accessTokenGenerator;
        }

        public async Task<Account> Authenticate(string email, string password)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(email);
            if (account == null || !VerifyPassword(account.Password, password))
            {
                return null;
            }
            return account;
        }

        public async Task<string> GenerateJwtToken(Account account)
        {
            var claims = new List<Claim>
        {
            new Claim("id", account.Id.ToString()),  
            new Claim("email", account.Email), 
            new Claim("profile-picture", account.ProfilePicture ?? string.Empty),
            new Claim("role", account.RoleName), 
            new Claim("username", account.Username) 
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.Lifetime);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string storedPassword, string providedPassword)
        {
            return storedPassword == providedPassword;
        }
        public async Task<string> GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/api/auth/google-callback"
            };
            return "/api/auth/google-signin";
        }

        public async Task<Result<object>> GoogleCallback(string token)
        {
            try
            {
                var infoDecodeAccessToken = await _googleOauthService.DecodeAccessToken(token);
                GoogleOauthDecode decode = infoDecodeAccessToken?.Data as GoogleOauthDecode;
                if (decode == null)
                {
                    return new Result<object>
                    {
                        Error = 1,
                        Message = "Invalid token",
                        Data = null
                    };
                }
                    var user = await _accountRepository.GetAccountByEmailAsync(decode.Email);
                    if (user == null)
                    {
                        LoginUserDTO loginDto = new LoginUserDTO()
                        {
                            Password = "",
                            Email = decode.Email,
                            Role = RoleEnum.Member.ToString()
                        };

                        var newUser = new Account
                        {
                            Id = Guid.NewGuid(),
                            Email = decode.Email,
                            Username = decode.FullName,
                            Password = "",
                            RoleName = loginDto.Role,
                            ProfilePicture = "",
                            Address = "",
                            Bio = "",
                            PhoneNumber = "",
                            Type = "Google"
                        };
                        string newRefreshToken = _refreshTokenGenerator.GenerateToken(loginDto);
                        string newAccessToken = _accessTokenGenerator.GenerateToken(loginDto);

                        newUser.RefreshToken = newRefreshToken;
                        await _accountRepository.AddAsync(newUser);

                        return new Result<object>
                        {
                            Error = 0,
                            Message = "Login Successfully",
                            Data = new LoginResponse<LoginData>(newAccessToken, newRefreshToken,
                                new LoginData(newUser.Id, newUser.Username, newUser.RoleName, newUser.ProfilePicture, newUser.Email, newUser.Type))
                        };
                    }

                    if (user.Type == null || user.Type == "Local")
                    {
                        return new Result<object>
                        {
                            Error = 1,
                            Message = "Account had registered by local, please login by local!",
                            Data = null
                        };
                    }

                    LoginUserDTO login = new LoginUserDTO()
                    {
                        Password = "",
                        Email = decode.Email,
                        Role = user.RoleName
                    };

                    string refreshToken = _refreshTokenGenerator.GenerateToken(login);
                    string accessToken = _accessTokenGenerator.GenerateToken(login);

                    user.RefreshToken = refreshToken;
                    await _accountRepository.Update(user);

                    return new Result<object>
                    {
                        Error = 0,
                        Message = "Login successfully",
                        Data = new LoginResponse<LoginData>(accessToken, refreshToken,
                            new LoginData(user.Id, user.Username, user.RoleName, user.ProfilePicture, user.Email, user.Type))
                    };
                }
    catch (Exception ex)
            {
                return new Result<object>
                {
                    Error = 1,
                    Message = ex.Message,
                    Data = null
                };
            }
        }


        public async Task SaveUserInformation(string email, string name)
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = name,
                Password = "DefaultPassword123!", // Set a default password or handle password securely
                RoleName = "User",
                PhoneNumber = "0000000000" // Set a default phone number or handle it appropriately
            };
            await _accountRepository.AddAsync(account);

        }
    }
}
