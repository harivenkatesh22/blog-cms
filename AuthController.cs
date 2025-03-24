using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogCMSBackend.Models;
using BlogCMSBackend.Data;
using BlogCMSBackend.DTOs;

namespace BlogCMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BlogCmsContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BlogCmsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/signup
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] UserDto userDto)
        {
            if (userDto == null ||
                string.IsNullOrWhiteSpace(userDto.Email) ||
                string.IsNullOrWhiteSpace(userDto.Password) ||
                string.IsNullOrWhiteSpace(userDto.FirstName) ||
                string.IsNullOrWhiteSpace(userDto.LastName) ||
                string.IsNullOrWhiteSpace(userDto.Country))
            {
                return BadRequest("Email, Password, FirstName, LastName and Country are required.");
            }

            // Use trimmed lower-case email for lookup.
            if (_context.Users.Any(u => u.Email!.Trim().ToLower() == userDto.Email!.Trim().ToLower()))
                return BadRequest("User already exists.");

            var user = new User
            {
                Email = userDto.Email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Role = "Blogger", // Default role for new signups.
                FirstName = userDto.FirstName.Trim(),
                LastName = userDto.LastName.Trim(),
                Country = userDto.Country.Trim()
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Signup successful");
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null ||
                string.IsNullOrWhiteSpace(loginDto.Email) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Email and Password are required.");
            }

            string loginEmail = loginDto.Email.Trim().ToLower();
            var user = _context.Users.FirstOrDefault(u => u.Email!.Trim().ToLower() == loginEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials. Please enter correct email and password.");

            // Ensure required fields are not null
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Role))
                return StatusCode(500, "User data is incomplete.");

            var secretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("Missing JwtSettings:SecretKey");
            var issuer = _configuration["JwtSettings:Issuer"]
                ?? throw new InvalidOperationException("Missing JwtSettings:Issuer");
            var audience = _configuration["JwtSettings:Audience"]
                ?? throw new InvalidOperationException("Missing JwtSettings:Audience");
            if (!int.TryParse(_configuration["JwtSettings:ExpiryInMinutes"], out int expiryInMinutes))
                expiryInMinutes = 120;

            var key = Encoding.ASCII.GetBytes(secretKey);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),  // Using ! because it has been validated.
                new Claim(ClaimTypes.Role, user.Role!)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
}
