using AuthenticationService.Data;
using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthContext _dbContext;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthController(AuthContext dbContext, ITokenGenerator tokenGenerator)
    {
        _dbContext = dbContext;
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserCredential userCreds)
    {
        var user = _dbContext.UserCredentials.FirstOrDefault(u => u.Email == userCreds.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(userCreds.Password, user.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = _tokenGenerator.generateToken(user.Email, "User");

        return Ok(token);
    }
}