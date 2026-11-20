using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DazzleJewels.Data;
using DazzleJewels.Models;

namespace Dazzle_Jewels.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Check whether a user is logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Allow access only to administrators
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Retrieve dashboard statistics
            var dashboard = new AdminDashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),

                TotalOrders = await _context.Orders.CountAsync(),

                TotalCustomers = await _context.Users
                    .CountAsync(u => u.Role == "Customer"),

                TotalOrderValue = await _context.Orders
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0
            };

            return View(dashboard);
        }
    }
}