using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311POE.Data;
using PROG7311POE.Models;
using PROG7311POE.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace PROG7311POE.Controllers
{
    [Route("[controller]")]
    public class FarmerController : Controller
    {
        /// <summary>
        /// Access to SQL lite Db & context.
        /// </summary>
        private readonly MyDbContext _myDbContext;

        /// <summary>
        /// Default contrustor with SQL-lite DB context & access.
        /// </summary>
        /// <param name="context">SQL-lite DB context</param>
        public FarmerController(MyDbContext context)
        {
            _myDbContext = context;
        }

        /// <summary>
        /// Opens View
        /// </summary>
        /// <returns></returns>
        [HttpGet("Add")]
        public IActionResult Add()
        {
            var user = User.Identity.IsAuthenticated ? User : null;
            if (user != null)
            {
                var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                Console.WriteLine("User Roles: " + string.Join(", ", roles));
            }

            return View();
        }

        /// <summary>
        /// Employee can add a farmer with essential details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public IActionResult Add(FarmerUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = model.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Email = model.Email,
                    Role = "Farmer" // Setting the role as Farmer
                };

                _myDbContext.Users.Add(user);
                _myDbContext.SaveChanges();

                var savedUser = _myDbContext.Users.SingleOrDefault(u => u.Username == model.Username);
                if (savedUser != null)
                {
                    var farmer = new Farmer
                    {
                        UserID = savedUser.UserID,
                        FarmName = model.FarmName,
                        Location = model.Location
                    };

                    _myDbContext.Farmers.Add(farmer);
                    _myDbContext.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
    }
}
// 💻🌸✨💖 --<< End of File >>-- 💖✨🌸💻 