using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RestauranteApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            var adminUser = _config["AdminCredentials:Username"];
            var adminPass = _config["AdminCredentials:Password"];

            if (dto.Username != adminUser || dto.Password != adminPass)
                return Unauthorized(new { message = "Credenciales inválidas." });

            var expiresInHours = double.Parse(_config["Jwt:ExpiresInHours"] ?? "8");
            var expiresAt = DateTime.UtcNow.AddHours(expiresInHours);
            var token = GenerateToken("Admin", expiresAt);

            return Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Role = "Admin"
            });
        }

        private string GenerateToken(string role, DateTime expiresAt)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
