﻿using Eventa_BusinessObject.DTOs.Login;
using Eventa_BusinessObject;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Eventa_BusinessObject.Entities;

namespace Eventa_Repositories.Implements
{
    public class AccessTokenGenerator
    {
        private readonly TokenGenerator _tokenGenerators;
        private readonly JwtSettings _jwtSettings;
        public AccessTokenGenerator(TokenGenerator tokenGenerators, IOptions<JwtSettings> jwtSettings)
        {
            _tokenGenerators = tokenGenerators;
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(LoginUserDTO userDto)
        {
            List<Claim> claims = new() {
            new Claim("id",userDto.AccountId.ToString()),
            new Claim("email",userDto.Email),
            new Claim("profilePicture",userDto.Picture),
            new Claim("role",userDto.Role),
            new Claim("username", userDto.Username)
        };
            return _tokenGenerators.GenerateToken(_jwtSettings.AccessSecretToken, _jwtSettings.Issuer, _jwtSettings.Audience, _jwtSettings.AccessTokenExpMinute, claims);
        }
    }
}
