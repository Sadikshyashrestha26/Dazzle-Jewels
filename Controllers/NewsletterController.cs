using DazzleJewels.Data;
using DazzleJewels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using DazzleJewels.Services;
namespace DazzleJewels.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public NewsletterController(
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                !new EmailAddressAttribute().IsValid(email.Trim()))
            {
                TempData["NewsletterMessage"] =
                    "Please enter a valid email address.";

                return RedirectToAction("Index", "Home");
            }

            email = email.Trim().ToLowerInvariant();

            var existingSubscriber = await _context.NewsletterSubscribers
                .AnyAsync(s => s.Email == email);

            if (existingSubscriber)
            {
                TempData["NewsletterMessage"] =
                    "You are already subscribed!";

                return RedirectToAction("Index", "Home");
            }

            var subscriber = new NewsletterSubscriber
            {
                Email = email,
                SubscribedAt = DateTime.UtcNow
            };

            _context.NewsletterSubscribers.Add(subscriber);

            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendWelcomeEmailAsync(email);

                TempData["NewsletterMessage"] =
                    "Thank you for subscribing! A welcome email has been sent.";
            }
            catch (Exception)
            {
                TempData["NewsletterMessage"] =
                    "You are subscribed successfully, but we could not send your welcome email.";
            }

            return RedirectToAction("Index", "Home");

            return RedirectToAction("Index", "Home");
        }
    }
}