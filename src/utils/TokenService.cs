namespace API.src.utils
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    public static class TokenService
    {
        public static string GenerateToken(string email)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhc2QiLCJqdGkiOiJkY2YxYTMzMi04ZGZlLTQyZTgtOGNhYS0xOTJhMjY5YjlkNTAiLCJleHAiOjE3NjU0NzM1MzgsImlzcyI6InN0YWZmZ2Vhci5jb20iLCJhdWQiOiJzdGFmZmdlYXIuY29tIn0.hbrsdWo7hXgXV7xfM15ulmY75pO7xde-yH9B049anEc")); //TODO: Move to config
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 

            var token = new JwtSecurityToken(
                issuer: "staffgear.com",
                audience: "staffgear.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}