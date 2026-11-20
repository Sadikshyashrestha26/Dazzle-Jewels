
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DazzleJewels.Data;
using DazzleJewels.Models;

namespace Dazzle_Jewels.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // Registration page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handle registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string fullName,
            string email,
            string password,
            string? phoneNumber,
            string? address)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please fill in all required fields.";
                return View();
            }

            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                ViewBag.Error = "This email is already registered.";
                return View();
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address,
                Role = "Customer"
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Registration successful! Please log in.";

            return RedirectToAction("Login");
        }

        // Login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handle login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Home");
        }
    }
}
