
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Dazzle_Jewels.Models;
using DazzleJewels.Models;
using DazzleJewels.Data;

namespace Dazzle_Jewels.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Homepage
    public IActionResult Index()
    {
        return View();
    }

    // Shop page
    public async Task<IActionResult> Shop()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
