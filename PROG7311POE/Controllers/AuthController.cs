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
    public class AuthController : Controller
    {
        /// <summary>
        /// Access SQL-Lite database.
        /// </summary>
        private readonly MyDbContext _myDbContext;

        /// <summary>
        /// Default contrustor with SQL-lite DB context & access.
        /// </summary>
        /// <param name="context">SQL-lite DB context</param>
        public AuthController(MyDbContext context)
        {
            _myDbContext = context;
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

                // Retrieve the UserID after saving the user, to get the db auto-generated UserID
                var savedUser = _myDbContext.Users.SingleOrDefault(u => u.Username == model.Username);
                if (savedUser != null && model.Role == "Farmer")
                {
                    var farmer = new Farmer
                    {
                        UserID = savedUser.UserID,  // Get the db auto-generated UserID
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

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _myDbContext.Users.SingleOrDefault(u => u.Username == model.Username);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }
    }
}