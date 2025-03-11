using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string generateToken(string email)
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SecretKey") ?? "This_is_my_super_secret_key_12345678910!";
            var issuer = Environment.GetEnvironmentVariable("JWT_Issuer");
            var audience = Environment.GetEnvironmentVariable("JWT_Audience");
            var expiryMinutes = Environment.GetEnvironmentVariable("JWT_ExpiryMinutes");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiryMinutes))
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}