using FinalProject_E_Wallet.Data;
using FinalProject_E_Wallet.Models.ViewModels;
using FinalProject_E_Wallet.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_E_Wallet.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == model.Email || u.PhoneNumber == model.PhoneNumber);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email or Phone number already exists");
                TempData["Error"] = "Email or Phone number already exists";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["Success"] = "User Registered Successfully!";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Registration Failed!";
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                TempData["Success"] = "Login Success!";
                return RedirectToAction("Dashboard", "Wallet");
            }
            ModelState.AddModelError("", "Invalid email or password");
            TempData["Error"] = "Invalid email or password";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Clear();
            TempData["Success"] = "Logout Successful!";
            return RedirectToAction("Login");
        }
    }
}
