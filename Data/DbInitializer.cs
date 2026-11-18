using Microsoft.EntityFrameworkCore;
using DazzleJewels.Models;

namespace DazzleJewels.Data;

    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Prevent inserting duplicate sample records
            if (await context.Categories.AnyAsync())
            {
                return;
            }

            // 1. Insert Categories
            var categories = new List<Category>
            {
                new Category { CategoryName = "Earrings", Description = "Elegant earrings for every occasion." },
                new Category { CategoryName = "Necklaces", Description = "Beautiful necklaces and chains." },
                new Category { CategoryName = "Rings", Description = "Stylish rings for everyday wear." },
                new Category { CategoryName = "Bracelets", Description = "Fashionable bracelets and bangles." },
                new Category { CategoryName = "Pendants", Description = "Decorative pendants and charms." }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // 2. Insert Users
            var users = new List<User>
            {
                new User { FullName = "Aarav Sharma", Email = "aarav@example.com", PasswordHash = "DEMO_HASH_1" },
                new User { FullName = "Sita Thapa", Email = "sita@example.com", PasswordHash = "DEMO_HASH_2" },
                new User { FullName = "Rohan Karki", Email = "rohan@example.com", PasswordHash = "DEMO_HASH_3" },
                new User { FullName = "Anisha Rai", Email = "anisha@example.com", PasswordHash = "DEMO_HASH_4" },
                new User { FullName = "Nisha Gurung", Email = "nisha@example.com", PasswordHash = "DEMO_HASH_5" }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // 3. Insert Products
            var products = new List<Product>
            {
                new Product { ProductName = "Golden Pearl Earrings", Description = "Elegant pearl earrings with a golden finish.", Price = 1299, StockQuantity = 20, CategoryId = categories[0].CategoryId },
                new Product { ProductName = "Classic Silver Necklace", Description = "Minimal silver necklace for everyday styling.", Price = 1899, StockQuantity = 15, CategoryId = categories[1].CategoryId },
                new Product { ProductName = "Rose Gold Ring", Description = "Beautiful rose gold adjustable ring.", Price = 999, StockQuantity = 25, CategoryId = categories[2].CategoryId },
                new Product { ProductName = "Crystal Charm Bracelet", Description = "Stylish bracelet decorated with crystal charms.", Price = 1499, StockQuantity = 18, CategoryId = categories[3].CategoryId },
                new Product { ProductName = "Heart Shape Pendant", Description = "Delicate heart-shaped pendant.", Price = 799, StockQuantity = 30, CategoryId = categories[4].CategoryId }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // 4. Insert Orders
            var orders = new List<Order>
            {
                new Order { UserId = users[0].UserId, TotalAmount = 1299, Status = "Delivered" },
                new Order { UserId = users[1].UserId, TotalAmount = 1899, Status = "Pending" },
                new Order { UserId = users[2].UserId, TotalAmount = 999, Status = "Shipped" },
                new Order { UserId = users[3].UserId, TotalAmount = 1499, Status = "Processing" },
                new Order { UserId = users[4].UserId, TotalAmount = 799, Status = "Delivered" }
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            // 5. Insert Order Items
            var orderItems = new List<OrderItem>
            {
                new OrderItem { OrderId = orders[0].OrderId, ProductId = products[0].ProductId, Quantity = 1, UnitPrice = 1299 },
                new OrderItem { OrderId = orders[1].OrderId, ProductId = products[1].ProductId, Quantity = 1, UnitPrice = 1899 },
                new OrderItem { OrderId = orders[2].OrderId, ProductId = products[2].ProductId, Quantity = 1, UnitPrice = 999 },
                new OrderItem { OrderId = orders[3].OrderId, ProductId = products[3].ProductId, Quantity = 1, UnitPrice = 1499 },
                new OrderItem { OrderId = orders[4].OrderId, ProductId = products[4].ProductId, Quantity = 1, UnitPrice = 799 }
            };

            context.OrderItems.AddRange(orderItems);
            await context.SaveChangesAsync();
        }
    }

