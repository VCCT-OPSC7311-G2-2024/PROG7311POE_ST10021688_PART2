/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG7311POE.Data;
using PROG7311POE.Models;
using PROG7311POE.ViewModels;
using System.Linq;

namespace PROG7311POE.Controllers
{
    [Authorize(Policy = "EmployeePolicy")] // Only employees can access this controller
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        private readonly MyDbContext _context;

        public EmployeeController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("AddFarmer")]
        public IActionResult AddFarmer()
        {
            return View();
        }

        [HttpPost("AddFarmer")]
        public IActionResult AddFarmer(FarmerUserViewModel model)
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

                _context.Users.Add(user);
                _context.SaveChanges();

                var savedUser = _context.Users.SingleOrDefault(u => u.Username == model.Username);
                if (savedUser != null)
                {
                    var farmer = new Farmer
                    {
                        UserID = savedUser.UserID,
                        FarmName = model.FarmName,
                        Location = model.Location
                    };

                    _context.Farmers.Add(farmer);
                    _context.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
    }
}
// 💻🌸✨💖 --<< End of File >>-- 💖✨🌸💻*/