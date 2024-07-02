using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace NestF.Infrastructure.Utils;

public static class JwtUtil
{
    public static string GenerateJwt(Dictionary<string, object> claims,
                                     DateTime from,
                                     int minuteValid,
                                     IConfiguration config,
                                     bool useHmacSha512 = false,
                                     string? secretKey = null)
        {
            var encodedKey = Encoding.UTF8.GetBytes(secretKey ?? config["Jwt:Key"]!);
            var securityKey = new SymmetricSecurityKey(encodedKey);
            var credentials = new SigningCredentials(securityKey,
                                                     useHmacSha512 ? SecurityAlgorithms.HmacSha512 : SecurityAlgorithms.HmacSha256);
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"],
                Claims = claims,
                IssuedAt = from,
                Expires = from.AddMinutes(minuteValid),
                SigningCredentials = credentials
            };
            var handler = new JsonWebTokenHandler
            {
                SetDefaultTimesOnTokenCreation = false
            };
            return handler.CreateToken(descriptor);
        }
    
        public static async Task<bool> VerifyJwt(string token,
                                     IConfiguration config,
                                     bool useHmacSha512 = false,
                                     string? secretKey = null)
        {
            var encodedKey = Encoding.UTF8.GetBytes(secretKey ?? config["Jwt:Key"]!);
            var securityKey = new SymmetricSecurityKey(encodedKey);
            var validateParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = securityKey
            };
            var handler = new JsonWebTokenHandler();
            try
            {
                var result = await handler.ValidateTokenAsync(token, validateParams);
                var jwt = (JsonWebToken) result.SecurityToken;
                return jwt != null &&
                       jwt.Alg.Equals(useHmacSha512 ? SecurityAlgorithms.HmacSha512 : SecurityAlgorithms.HmacSha256,
                           StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
}