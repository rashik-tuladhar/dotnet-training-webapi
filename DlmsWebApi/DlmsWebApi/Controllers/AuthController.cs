using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DlmsWebApi.Repository.Models;

namespace LibrarySystem.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static readonly List<RefreshToken> RefreshTokenDatabase = new List<RefreshToken>();

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "admin" && request.Password == "password123")
            {
                var accessToken = GenerateAccessToken(request.Username, "Admin");
                var refreshToken = GenerateRefreshToken(request.Username);

                lock (RefreshTokenDatabase)
                {
                    RefreshTokenDatabase.Add(refreshToken);
                }

                return Ok(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token
                });
            }

            return Unauthorized(new { Message = "Invalid username or password" });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] TokenApiRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token structure");
            }

            var username = principal.Identity?.Name;

            RefreshToken? savedRefreshToken;
            lock (RefreshTokenDatabase)
            {
                savedRefreshToken = RefreshTokenDatabase.FirstOrDefault(x => x.Token == request.RefreshToken);
            }

            if (savedRefreshToken == null || savedRefreshToken.Username != username || !savedRefreshToken.IsActive)
            {
                return BadRequest("Invalid or expired refresh token");
            }

            lock (RefreshTokenDatabase)
            {
                savedRefreshToken.IsUsed = true;
            }

            var newAccessToken = GenerateAccessToken(username!, "Admin");
            var newRefreshToken = GenerateRefreshToken(username!);

            lock (RefreshTokenDatabase)
            {
                RefreshTokenDatabase.Add(newRefreshToken);
            }

            return Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        [HttpPost("revoke")]
        public IActionResult Revoke([FromBody] string token)
        {
            RefreshToken? storedToken;
            lock (RefreshTokenDatabase)
            {
                storedToken = RefreshTokenDatabase.FirstOrDefault(x => x.Token == token);
            }

            if (storedToken != null)
            {
                lock (RefreshTokenDatabase)
                {
                    storedToken.IsRevoked = true;
                }
                return Ok(new { Message = "Token successfully revoked." });
            }

            return BadRequest("Token not found.");
        }

        private string GenerateAccessToken(string username, string role)
        {
            var keyString = _configuration["Jwt:Key"] ?? "SuperSecretDefaultSecurityKey12345678";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "DlmsWebApiAuthority",
                audience: _configuration["Jwt:Audience"] ?? "DlmsWebApiClients",
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string username)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Username = username,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsUsed = false,
                IsRevoked = false
            };
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var keyString = _configuration["Jwt:Key"] ?? "SuperSecretDefaultSecurityKey12345678";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            
            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class TokenApiRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
