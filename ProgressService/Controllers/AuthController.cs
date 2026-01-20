using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProgressService.Models.Dto;
using ProgressService.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProgressService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config = config;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var admin = await _authService.ValidateAdminAsync(dto.UserName, dto.Password);

            if (admin == null)
                return Unauthorized("Invalid credentials");

            // Claims
            var claims = new List<Claim>
            {
                new Claim("adminId", admin.Value.AdminId.ToString()),
                new Claim("isAdmin", admin.Value.IsAdmin ? "true" : "false"),
                new Claim(ClaimTypes.Name, admin.Value.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            return Ok(new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
