using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieActorAPI.Data;
using MovieActorAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieActorAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MovieActorDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(MovieActorDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/User
        [HttpPost]
        public ActionResult<User> CreateUser(UserTemplete user)
        {
            if (_context.Users.Any(u => u.UserName == user.UserName))
            {
                return BadRequest("Username already exists");
            }
            User user1 = new User { Email=user.Email, Password=user.Password, UserName=user.UserName};
            _context.Users.Add(user1);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUser), new { id = user1.UserId }, user1);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            var user = await _context.Users.ToListAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> LogIn([FromBody] Login login)
        {
            // Verify User
            var User = _context.Users.FirstOrDefault(u => u.UserName == login.UserName);
            if (User == null)
            {
                return BadRequest("Incorrect UserName");
            }
            // Verify password
            if (User.Password != login.Password)
            {
                return BadRequest("Incorrect password");
            }

            // Create claims for the user
            var claims = new[]
            {
                new Claim("UserId", User.UserId.ToString()),
            };

            // Generate the security key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // Generate the signing credentials
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Generate the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // Set the token expiration
                signingCredentials: signingCredentials
            );

            return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token), User = User });
        }

    }
}
