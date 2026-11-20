
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DazzleJewels.Data;

namespace Dazzle_Jewels.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return View(categories);
        }

        // Display products of a selected category
        [HttpGet("Category/Products/{name}")]
        public async Task<IActionResult> Products(
            string name, string? search)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c =>
                    c.CategoryName.ToLower() == name.ToLower());

            if (category == null)
            {
                return NotFound();
            }

            // Search products within the selected category
            if (!string.IsNullOrWhiteSpace(search))
            {
                category.Products = category.Products
                    .Where(p => p.ProductName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Search = search;

            return View("Products", category);
        }
    }
}
