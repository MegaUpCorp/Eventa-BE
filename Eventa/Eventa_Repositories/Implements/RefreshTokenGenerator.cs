using Eventa_BusinessObject;
using Eventa_BusinessObject.DTOs.Login;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Repositories.Implements
{
    public class RefreshTokenGenerator
    {
        private readonly TokenGenerator _tokenGenerators;
        private readonly JwtSettings _jwtSettings;
        public RefreshTokenGenerator(TokenGenerator tokenGenerators, IOptions<JwtSettings> jwtSettings)
        {
            _tokenGenerators = tokenGenerators;
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(LoginUserDTO userDto)
        {

            List<Claim> claims = new() {
        new Claim(ClaimTypes.Email, userDto.Email),
        new Claim(ClaimTypes.Role, userDto.Role)
    };
            return _tokenGenerators.GenerateToken(_jwtSettings.RefreshSecretToken, _jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.RefreshTokenExpMinute, claims);
        }
    }
}
