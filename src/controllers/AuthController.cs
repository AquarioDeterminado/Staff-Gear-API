
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.src.auth;           // User
using API.src.models;        // AdventureWorksContext
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.src.controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AdventureWorksContext _db;
        private readonly IConfiguration _cfg;

        public AuthController(AdventureWorksContext db, IConfiguration cfg)
        {
            _db = db;
            _cfg = cfg;
        }

        public class LoginRequest
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username e password são obrigatórios.");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == req.Username);

            if (user is null || !user.IsActive)
                return Unauthorized("Utilizador inválido ou inativo.");

            var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!ok) return Unauthorized("Credenciais inválidas.");

            // Claims base
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.UserId.ToString()),
            };

            // Claim de role (string)
            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role)); // ex.: "HR", "Employee"
            }

            // Config JWT
            var jwt = _cfg.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresMinutes = int.TryParse(jwt["ExpireMinutes"], out var mins) ? mins : 60;

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                access_token = tokenStr,
                role = user.Role,
                user_id = user.UserId,
                employee_id = user.EmployeeId
            });
        }
    }
}