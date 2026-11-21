
using Microsoft.AspNetCore.Mvc;
using DazzleJewels.Data;
using DazzleJewels.Models;

namespace Dazzle_Jewels.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrackingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackPageView(
            [FromBody] PageView pageView)
        {
            if (pageView == null ||
                string.IsNullOrWhiteSpace(pageView.PagePath) ||
                !pageView.PagePath.StartsWith("/") ||
                pageView.PagePath.Length > 500)
            {
                return BadRequest();
            }

            var newPageView = new PageView
            {
                PagePath = pageView.PagePath,
                ViewedAt = DateTime.UtcNow
            };

            _context.PageViews.Add(newPageView);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Page view recorded" });
        }
    }
}
