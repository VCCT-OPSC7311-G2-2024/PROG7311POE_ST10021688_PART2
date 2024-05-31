using Microsoft.AspNetCore.Mvc;
using PROG7311POE.Data;
using PROG7311POE.Models;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PROG7311POE.ViewModels;

namespace PROG7311POE.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        /// <summary>
        /// Access SQL-Lite database.
        /// </summary>
        private readonly MyDbContext _myDbContext;
        private readonly IConfiguration _myAuthConfiguration;

        /// <summary>
        /// Default contructor with SQL-lite DB context & access. Also authentication access to JWT in config.
        /// 
        /// </summary>
        /// <param name="context">SQL-lite DB context</param>
        public AuthController(MyDbContext context, IConfiguration authConfig)
        {
            _myDbContext = context;
            _myAuthConfiguration = authConfig;
        }

        /// <summary>
        /// Opens Register View.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Registers new user. Will add extra details for farmer-users.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = model.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Email = model.Email,
                    Role = model.Role
                };

                _myDbContext.Users.Add(user);
                _myDbContext.SaveChanges();

                // Retrieve the UserID after saving the user
                var savedUser = _myDbContext.Users.SingleOrDefault(u => u.Username == model.Username);
                if (savedUser != null && model.Role == "Farmer")
                {
                    var farmer = new Farmer
                    {
                        UserID = savedUser.UserID,
                        FarmName = model.FarmName,
                        Location = model.Location
                    };

                    _myDbContext.Farmers.Add(farmer);
                    _myDbContext.SaveChanges();
                }

                return RedirectToAction("Login", "Auth");
            }
            return View(model);
        }

        /// <summary>
        /// Open Login View
        /// </summary>
        /// <returns></returns>
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Authorizes user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _myDbContext.Users.SingleOrDefault(u => u.Username == model.Username);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    var token = GenerateJwtToken(user);
                    // Store token in cookie for subsequent requests
                    Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true, Secure = true });
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        /// <summary>
        /// Generates security token, after successfull login.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_myAuthConfiguration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _myAuthConfiguration["Jwt:Issuer"],
                audience: DetermineAudience(user),
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Limits access to views based on roles: farmer or employee.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string DetermineAudience(User user)
        {
            // Determine the audience based on user role or other criteria
            return user.Role == "Farmer" ? _myAuthConfiguration["Jwt:Audiences:0"] : _myAuthConfiguration["Jwt:Audiences:1"];
        }
    }
}
// 💻🌸✨💖 --<< End of File >>-- 💖✨🌸💻