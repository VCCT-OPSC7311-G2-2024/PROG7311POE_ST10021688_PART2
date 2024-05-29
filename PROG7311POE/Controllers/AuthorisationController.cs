using Microsoft.AspNetCore.Mvc;
using PROG7311POE.Data;
using PROG7311POE.Models;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace PROG7311POE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorisationController : ControllerBase
    {
        private readonly MyDbContext _myDbContext;
        private static string activeUser;
        // Access JWT authentication 
        private readonly IConfiguration _myConfiguration;

        public AuthorisationController(MyDbContext context, IConfiguration configuration)
        {
            _myDbContext = context;
            _myConfiguration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var myUser = _myDbContext.Users.SingleOrDefault(u => u.Username == login.Username);

            if (myUser == null || !BCrypt.Net.BCrypt.Verify(login.Password, myUser.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            // Check if there's an active user
            if (!string.IsNullOrEmpty(activeUser))
            {
                return Unauthorized("Another user is already logged in");
            }

            activeUser = myUser.Username;

            var tokenHandler = new JwtSecurityTokenHandler();
            var myKey = Encoding.ASCII.GetBytes(_myConfiguration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, myUser.Username.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(myKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _myConfiguration["Jwt:Issuer"],
                Audience = _myConfiguration["Jwt:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (activeUser == User.Identity.Name)
            {
                activeUser = null;
                return Ok("Logged out successfully");
            }

            return Unauthorized("You're not supposed to be here....");
        }
    }
}