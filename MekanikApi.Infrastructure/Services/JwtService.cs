using Microsoft.Extensions.Logging;
using MekanikApi.Application.Interfaces;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using MekanikApi.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;


namespace MekanikApi.Infrastructure.Services
{
    public class JwtService(ILogger<JwtService> logger) : IJwtService
    {
        private readonly ILogger<JwtService> _logger = logger;

        public ClaimsPrincipal GetTokenPrincipal(string accessToken)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTKEY")));

            var validation = new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateLifetime = true,
                ValidateActor = true,
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWTISSUER"),
                ValidateAudience = false,
                ValidAudience = Environment.GetEnvironmentVariable("JWTAUDIENCE")
            };
            try
            {
                return new JwtSecurityTokenHandler().ValidateToken(accessToken, validation, out _);
            }
            catch (Exception ex)
            {
                _logger.LogError("Token validation error: {ex}", ex.Message);
                return null;
            }
        }

        public static string GenerateRefreshToken(string email)
        {
            IEnumerable<Claim> claims =
            [
                new Claim(ClaimTypes.Name ,email),
            ];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTKEY")));

            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var securityToken = new JwtSecurityToken(
                claims: claims,
                 expires: DateTime.UtcNow.AddMinutes(3),
                 issuer: Environment.GetEnvironmentVariable("JWTISSUER"),
                 audience: Environment.GetEnvironmentVariable("JWTAUDIENCE"),
                 signingCredentials: signingCred
              );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }

        public static string GenerateAccessTokenAsync(string email, Guid id, IList<string> roles)
        {
            List<Claim> claims =
            [
                new Claim(ClaimTypes.Name ,email),
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            ];

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTKEY")));

            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var securityToken = new JwtSecurityToken(
                claims: claims,
                 expires: DateTime.UtcNow.AddMinutes(1),
                 issuer: Environment.GetEnvironmentVariable("JWTISSUER"),
                 audience: Environment.GetEnvironmentVariable("JWTAUDIENCE"),
                 signingCredentials: signingCred
              );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }
    }
}