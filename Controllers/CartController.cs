
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using DazzleJewels.Data;
using DazzleJewels.Models;

namespace DazzleJewels.Controllers
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
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

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
            return View(GetCart());
        }

        // Read cart from session
        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");

            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson)
                    ?? new List<CartItem>();
        }

        // Save cart to session
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart",
                JsonSerializer.Serialize(cart));
        }

        // Checkout page
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction("Index");

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
                return RedirectToAction("Login", "Account");

            ViewBag.User = user;

            return View(cart);
        }

        // Dummy payment gateway page
        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction("Index");

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
                return RedirectToAction("Login", "Account");

            ViewBag.User = user;
            ViewBag.TotalAmount = cart.Sum(item =>
                item.Price * item.Quantity);

            return View(cart);
        }

        // Process simulated payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(
            string paymentResult,
            string paymentMethod = "Demo Payment")
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction("Index");

            if (paymentResult != "success" && paymentResult != "failure")
                return BadRequest();

            var transactionId = "DZ-" +
                Guid.NewGuid().ToString("N")[..12].ToUpper();

            var paymentDate = DateTime.Now.ToString("dd MMM yyyy, hh:mm tt");

            // Calculate amount using current database prices
            var productIds = cart.Select(item => item.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            if (products.Count != cart.Count)
                return RedirectToAction("Index");

            decimal total = 0;

            foreach (var item in cart)
            {
                var product = products.First(p =>
                    p.ProductId == item.ProductId);

                total += product.Price * item.Quantity;
            }

            // Simulate failed payment
            if (paymentResult == "failure")
            {
                TempData["PaymentStatus"] = "Failed";
                TempData["PaymentMessage"] =
                    "Your payment could not be processed. Please try again.";
                TempData["TotalAmount"] = total.ToString("N2");
                TempData["TransactionId"] = transactionId;
                TempData["PaymentDate"] = paymentDate;
                TempData["PaymentMethod"] = paymentMethod;

                return RedirectToAction("Confirmation");
            }

            // Simulate successful payment
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // Check stock again before confirming the order
                foreach (var item in cart)
                {
                    var product = products.First(p =>
                        p.ProductId == item.ProductId);

                    if (product.StockQuantity < item.Quantity)
                    {
                        await transaction.RollbackAsync();

                        TempData["PaymentStatus"] = "Failed";
                        TempData["PaymentMessage"] =
                            $"Insufficient stock for {product.ProductName}. Please update your cart.";
                        TempData["TotalAmount"] = total.ToString("N2");
                        TempData["TransactionId"] = transactionId;
                        TempData["PaymentDate"] = paymentDate;
                        TempData["PaymentMethod"] = paymentMethod;

                        return RedirectToAction("Confirmation");
                    }
                }

                var order = new Order
                {
                    UserId = userId.Value,
                    TotalAmount = total,
                    OrderDate = DateTime.UtcNow,
                    Status = "Paid"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in cart)
                {
                    var product = products.First(p =>
                        p.ProductId == item.ProductId);

                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    _context.OrderItems.Add(orderItem);

                    product.StockQuantity -= item.Quantity;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Clear cart after successful payment
                HttpContext.Session.Remove("Cart");

                // Store only TempData-safe values
                TempData["PaymentStatus"] = "Success";
                TempData["PaymentMessage"] =
                    "Your payment has been processed successfully.";
                TempData["OrderId"] = order.OrderId;
                TempData["TotalAmount"] = total.ToString("N2");
                TempData["TransactionId"] = transactionId;
                TempData["PaymentDate"] = paymentDate;
                TempData["PaymentMethod"] = paymentMethod;

                return RedirectToAction("Confirmation");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Payment receipt / result page
        [HttpGet]
        public IActionResult Confirmation()
        {
            ViewBag.PaymentStatus =
                TempData["PaymentStatus"]?.ToString();

            ViewBag.PaymentMessage =
                TempData["PaymentMessage"]?.ToString();

            ViewBag.OrderId =
                TempData["OrderId"]?.ToString();

            ViewBag.TotalAmount =
                TempData["TotalAmount"]?.ToString();

            ViewBag.TransactionId =
                TempData["TransactionId"]?.ToString();

            ViewBag.PaymentDate =
                TempData["PaymentDate"]?.ToString();

            ViewBag.PaymentMethod =
                TempData["PaymentMethod"]?.ToString();

            if (ViewBag.PaymentStatus == null)
                return RedirectToAction("Index");

            return View();
        }

        // View user's previous orders
        [HttpGet]
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

        // Update cart quantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    cart.Remove(item);
                }
                else if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else if (quantity <= product.StockQuantity)
                {
                    item.Quantity = quantity;
                }
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Remove item from cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();

            cart.RemoveAll(item => item.ProductId == id);

            SaveCart(cart);

            return RedirectToAction("Index");
        }
    }
}
