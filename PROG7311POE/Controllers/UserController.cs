using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using PROG7311POE.Data;
using PROG7311POE.Models;


namespace PROG7311POE.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDbContext _MyDbContext;
        public UserController(MyDbContext context)
        {
            _MyDbContext = context;
        }

        /// <summary>
        /// open index view
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var users = await _MyDbContext.Users.ToListAsync();
            return View(users);
        }


        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _MyDbContext.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                _MyDbContext.Add(user);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _MyDbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            // Check if url-userID matches the userID
            if (id != user.UserID)
            {
                return NotFound();
            }

            // If model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Check databse for user
                    var existingUser = await _MyDbContext.Users.FindAsync(id);

                    // Error if user is not in database
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Update user
                    _MyDbContext.Entry(existingUser).CurrentValues.SetValues(user);

                    // Hash password
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                        user.Password = Convert.ToBase64String(hashedBytes);
                    }

                    // Save changes to the database
                    await _MyDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Show user table with updates 
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        private bool UserExists(int id)
        {
            return _MyDbContext.Users.Any(e => e.UserID == id);
        }

        /// <summary>
        /// Deletes a logged in user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _MyDbContext.Users
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                _MyDbContext.Users.Remove(user);
                await _MyDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        // ===== AUTHENTICATION FEATURE ====================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User userIn)
        {
            if (ModelState.IsValid)
            {
                // Check if the username or email is already taken (replace this with your actual validation logic)
                if (_MyDbContext.Users.Any(u => u.Username == userIn.Username || u.Email == userIn.Email))
                {
                    ModelState.AddModelError(string.Empty, "Username or email is already taken.");
                    return View(userIn);
                }
                //TODO: CAPTURE ROLE
                // Create new user
                User newUser = new User(userIn.Username,userIn.Email,userIn.Password,"role"); // Note: password hashing is done in the User constructor
                
                //Add new user to db
                _MyDbContext.Add(newUser);
                await _MyDbContext.SaveChangesAsync();

                // Redirect to login
                return RedirectToAction("Login");
            }

            return View(userIn);
        }


        // GET: User/Register
        public IActionResult Register()
        {
            return View();
        }


        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (ModelState.IsValid)
            {
                // Validate user 
                var user = await _MyDbContext.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == (model.Password));// TODO: hash passwsord input

                if (user != null)
                {
                    // Save user ID for databases entires later
                    //SignInManager.setActiveUser(user.UserId);

                    // Redirect to a secure area or dashboard after successful login
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }

            return View(model);
        }
    }
}
