using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CloudSeedApp
{
    public class JwtAuthenticationProcessor
    {
        private const string JWT_PURPOSE = "purpose";
        private ConfigurationProvider _configuration;
        private INowProvider _nowProvider;

        public JwtAuthenticationProcessor(
            ConfigurationProvider configuration,
            INowProvider nowProvider) {
            this._configuration = configuration;
            this._nowProvider = nowProvider;
        }

        public static TokenValidationParameters GetJwtAuthenticationTokenValidationParameters(string key) {
            return new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey (
                    Encoding.ASCII.GetBytes(key)
                ),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ValidateLifetime = true
            };
        }

        public string GenerateAuthenticationToken(
            User user,
            JwtAuthenticationPurposes purpose) {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(this._configuration.JWT_SIGNING_KEY);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new System.Security.Claims.ClaimsIdentity(
                    new List<Claim> {
                        new Claim(
                            JwtRegisteredClaimNames.Sub,
                            user.Id.ToString()
                        ),
                        new Claim(
                            JwtRegisteredClaimNames.Iat,
                            this._nowProvider.GetNowDateTimeOffset().ToUnixTimeSeconds().ToString()
                        ),
                        new Claim(
                            JWT_PURPOSE,
                            purpose.ToString()
                        )
                    }
                ),
                Expires = this._nowProvider.GetNowDateTimeOffset().AddHours(12).DateTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        keyBytes
                    ),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }

        public bool TryValidateAuthenticationToken(
            string token,
            JwtAuthenticationPurposes purpose,
            out JwtSecurityToken? validatedJwtToken) {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(this._configuration.JWT_SIGNING_KEY);

            try {
                var claimsPrincipal = jwtTokenHandler.ValidateToken(
                    token,
                    GetJwtAuthenticationTokenValidationParameters(this._configuration.JWT_SIGNING_KEY),
                    out SecurityToken validatedToken
                );

                var tokenPurpose = claimsPrincipal.FindFirst(JWT_PURPOSE)?.Value;
                if(tokenPurpose != purpose.ToString()) {
                    throw new InvalidOperationException("JWT token purposes do not match!");
                }

                validatedJwtToken = (JwtSecurityToken)validatedToken;
                return true;
            } catch 
            {
                validatedJwtToken = null;
                return false;
            }
        }
    }
}