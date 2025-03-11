using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserProfileService.Data;
using UserProfileService.Models;
using UserProfileService.Services;

namespace UserProfileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserProfileContext _context;
        private readonly KafkaProducer _kafkaProducer;
        private readonly string _kafkaTopic = "user-credentials";

        public UserController(UserProfileContext context)
        {
            _context = context;
            _kafkaProducer = new KafkaProducer("kafka:9092");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
                return BadRequest("Invalid user data.");

            var existingUser = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (existingUser)
                return Conflict("Email already exists.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Publish user credentials to Kafka
            var credentials = new { user.Email, user.Password };
            await _kafkaProducer.PublishUserCredentialsAsync(_kafkaTopic, credentials);

            return Ok(new { message = "User registered successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("profile")]
        //[Authorize] // Ensure only authenticated users can access
        public async Task<IActionResult> GetProfile()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token is missing.");

            var userEmail = GetEmailFromToken(token);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Invalid token.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return NotFound("User not found.");

            return Ok(new { user.FirstName, user.LastName, user.Email });
        }

        private string? GetEmailFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
    }
}
