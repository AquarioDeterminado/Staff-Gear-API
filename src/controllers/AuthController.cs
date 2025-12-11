using API.src.models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using API.src.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace API.src.controllers
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    [ApiController]
    [Route("api/v1/[controller]")]
    class AuthController : ControllerBase
    {
        private readonly AdventureWorksContext _db;
        private readonly IMapper _mapper;

        public AuthController(AdventureWorksContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var BusinessEntityID = _db.EmailAddress
                .Where(e => e.EmailAddress1 == request.Email)
                .Select(e => e.BusinessEntityID)
                .FirstOrDefault();

            var dbPass = _db.Password
                .Where(p => p.BusinessEntityID == BusinessEntityID)
                .Select(p => p.PasswordHash)
                .FirstOrDefault();
            
            if (dbPass == null || !BCrypt.Net.BCrypt.Verify(request.Password, dbPass)) return Unauthorized("Invalid email or password.");

            var newToken = TokenService.GenerateToken(request.Email);

            return Ok(new { AuthToken = newToken });
        }

        // [HttpGet("logout")]
        // [Authorize]
        // public async Task<IActionResult> Logout()
        // {
            
        // }
    }
}

