
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using DazzleJewels.Data;
using DazzleJewels.Models;

namespace Dazzle_Jewels.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add product to cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(
                p => p.ProductId == id);

            if (existingItem != null)
            {
                if (existingItem.Quantity < product.StockQuantity)
                    existingItem.Quantity++;
            }
            else if (product.StockQuantity > 0)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = 1
                });
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // Display cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Get cart from session
        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson)
                   ?? new List<CartItem>();
        }

        // Save cart to session
        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }

        // Display Checkout page
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCart();

            if (cart.Count == 0)
                return RedirectToAction("Index");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null)
                return RedirectToAction("Login", "Account");

            ViewBag.User = user;

            return View(cart);
        }

        // Place order with database transaction and rollback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCart();

            if (cart.Count == 0)
                return RedirectToAction("Index");

            // Start database transaction
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var products = new List<Product>();

                // Check stock availability
                foreach (var item in cart)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(
                            p => p.ProductId == item.ProductId);

                    if (product == null ||
                        product.StockQuantity < item.Quantity)
                    {
                        await transaction.RollbackAsync();

                        TempData["Error"] =
                            $"Insufficient stock for {item.ProductName}.";

                        return RedirectToAction("Index");
                    }

                    products.Add(product);
                }

                // Calculate total amount
                decimal total = products.Sum(p =>
                    p.Price * cart.First(
                        c => c.ProductId == p.ProductId).Quantity);

                // Create order
                var order = new Order
                {
                    UserId = userId.Value,
                    TotalAmount = total,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create order items and update stock
                foreach (var product in products)
                {
                    var cartItem = cart.First(
                        c => c.ProductId == product.ProductId);

                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.Price
                    };

                    _context.OrderItems.Add(orderItem);

                    product.StockQuantity -= cartItem.Quantity;
                }

                await _context.SaveChangesAsync();

                // Commit transaction if everything succeeds
                await transaction.CommitAsync();

                HttpContext.Session.Remove("Cart");

                TempData["OrderSuccess"] =
                    "Your order has been placed successfully!";

                return RedirectToAction("Confirmation");
            }
            catch
            {
                // Rollback all database changes if an error occurs
                await transaction.RollbackAsync();

                throw;
            }
        }

        // Order confirmation page
        public IActionResult Confirmation()
        {
            return View();
        }

        // Display customer's order history
        public async Task<IActionResult> MyOrders()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var orders = await _context.Orders
                .Where(o => o.UserId == userId.Value)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Update product quantity in cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(
            int id, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(
                p => p.ProductId == id);

            if (item == null)
                return NotFound();

            if (quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                    return NotFound();

                if (quantity > product.StockQuantity)
                {
                    TempData["Error"] =
                        $"Only {product.StockQuantity} items are available.";

                    return RedirectToAction("Index");
                }

                item.Quantity = quantity;
            }

            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // Remove product from cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(
                p => p.ProductId == id);

            if (item != null)
                cart.Remove(item);

            SaveCart(cart);

            return RedirectToAction("Index");
        }
    }
}
