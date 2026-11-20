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
    new Product
    {
        ProductName = "Golden Pearl Earrings",
        Description = "Elegant pearl earrings with a golden finish.",
        Price = 1299,
        StockQuantity = 20,
        ImageUrl = "/images/earrings.jpg",
        CategoryId = categories[0].CategoryId
    },

    new Product
    {
        ProductName = "Classic Silver Necklace",
        Description = "Minimal silver necklace for everyday styling.",
        Price = 1899,
        StockQuantity = 15,
        ImageUrl = "/images/necklace.jpg",
        CategoryId = categories[1].CategoryId
    },

    new Product
    {
        ProductName = "Rose Gold Ring",
        Description = "Beautiful rose gold adjustable ring.",
        Price = 999,
        StockQuantity = 25,
        ImageUrl = "/images/ring.jpg",
        CategoryId = categories[2].CategoryId
    },

    new Product
    {
        ProductName = "Crystal Charm Bracelet",
        Description = "Stylish bracelet decorated with crystal charms.",
        Price = 1499,
        StockQuantity = 18,
        ImageUrl = "/images/bracelet.jpg",
        CategoryId = categories[3].CategoryId
    },

    new Product
    {
        ProductName = "Heart Shape Pendant",
        Description = "Delicate heart-shaped pendant.",
        Price = 799,
        StockQuantity = 30,
        ImageUrl = "/images/pendant.jpg",
        CategoryId = categories[4].CategoryId
    }
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

    public static async Task AddMoreProductsAsync(ApplicationDbContext context)
    {
        var categories = await context.Categories
            .ToDictionaryAsync(c => c.CategoryName, c => c.CategoryId);

        var products = new List<Product>
    {
        // EARRINGS
        new Product { ProductName = "Classic Hoop Earrings", Description = "Timeless golden hoop earrings.", Price = 899, StockQuantity = 20, ImageUrl = "/images/earrings/pearl.jpg", CategoryId = categories["Earrings"] },
        new Product { ProductName = "Diamond Stud Earrings", Description = "Elegant sparkling stud earrings.", Price = 1599, StockQuantity = 15, ImageUrl = "/images/earrings/diamond.jpg", CategoryId = categories["Earrings"] },
        new Product { ProductName = "Floral Drop Earrings", Description = "Beautiful floral drop earrings.", Price = 1199, StockQuantity = 18, ImageUrl = "/images/earrings/floral.jpg", CategoryId = categories["Earrings"] },
        new Product { ProductName = "Butterfly Earrings", Description = "Cute butterfly-shaped earrings.", Price = 999, StockQuantity = 22, ImageUrl = "/images/earrings/butterfly.jpg", CategoryId = categories["Earrings"] },

        // NECKLACES
        new Product { ProductName = "Golden Chain Necklace", Description = "Simple and elegant golden chain.", Price = 1499, StockQuantity = 20, ImageUrl = "/images/necklace/chain.jpg", CategoryId = categories["Necklaces"] },
        new Product { ProductName = "Pearl Necklace", Description = "Classic necklace decorated with pearls.", Price = 1999, StockQuantity = 12, ImageUrl = "/images/necklace/pearl.jpg", CategoryId = categories["Necklaces"] },
        new Product { ProductName = "Layered Necklace", Description = "Trendy layered necklace for everyday wear.", Price = 1799, StockQuantity = 16, ImageUrl = "/images/necklace/layer.jpg", CategoryId = categories["Necklaces"] },
        new Product { ProductName = "Heart Necklace", Description = "Delicate necklace with a heart design.", Price = 1299, StockQuantity = 20, ImageUrl = "/images/necklace/heart.jpg", CategoryId = categories["Necklaces"] },

        // RINGS
        new Product { ProductName = "Solitaire Ring", Description = "Elegant solitaire-style ring.", Price = 1899, StockQuantity = 15, ImageUrl = "/images/ring/solitaire.jpg", CategoryId = categories["Rings"] },
        new Product { ProductName = "Pearl Ring", Description = "Beautiful ring with a pearl centerpiece.", Price = 1199, StockQuantity = 18, ImageUrl = "/images/ring/pearl.jpg", CategoryId = categories["Rings"] },
        new Product { ProductName = "Heart Design Ring", Description = "Charming heart-shaped ring.", Price = 999, StockQuantity = 20, ImageUrl = "/images/ring/heart.jpg", CategoryId = categories["Rings"] },

        // BRACELETS
        new Product { ProductName = "Tennis Bracelet", Description = "Elegant bracelet with sparkling stones.", Price = 2199, StockQuantity = 12, ImageUrl = "/images/bracelet/tennis.jpg", CategoryId = categories["Bracelets"] },
        new Product { ProductName = "Golden Chain Bracelet", Description = "Minimal golden chain bracelet.", Price = 1299, StockQuantity = 20, ImageUrl = "/images/bracelet/chain.jpg", CategoryId = categories["Bracelets"] },
        new Product { ProductName = "Pearl Charm Bracelet", Description = "Stylish bracelet with pearl charms.", Price = 1599, StockQuantity = 15, ImageUrl = "/images/bracelet/pearl.jpg", CategoryId = categories["Bracelets"] },
        new Product { ProductName = "Heart Charm Bracelet", Description = "Beautiful bracelet with heart charms.", Price = 1399, StockQuantity = 18, ImageUrl = "/images/bracelet/heart.jpg", CategoryId = categories["Bracelets"] },

        // PENDANTS
        new Product { ProductName = "Star Pendant", Description = "Elegant star-shaped pendant.", Price = 899, StockQuantity = 20, ImageUrl = "/images/pendant/star.jpg", CategoryId = categories["Pendants"] },
        new Product { ProductName = "Moon Pendant", Description = "Beautiful crescent moon pendant.", Price = 999, StockQuantity = 22, ImageUrl = "/images/pendant/moon.jpg", CategoryId = categories["Pendants"] },       
        new Product { ProductName = "Butterfly Pendant", Description = "Delicate butterfly-shaped pendant.", Price = 1099, StockQuantity = 18, ImageUrl = "/images/pendant/butterfly.jpg", CategoryId = categories["Pendants"] },
        new Product { ProductName = "Pearl Pendant", Description = "Simple pendant featuring a pearl.", Price = 1299, StockQuantity = 15, ImageUrl = "/images/pendant/pearl.jpg", CategoryId = categories["Pendants"] },


    };
        
        foreach (var product in products)
        {
            bool alreadyExists = await context.Products
                .AnyAsync(p => p.ProductName == product.ProductName);

            if (!alreadyExists)
            {
                context.Products.Add(product);
            }
        }

        await context.SaveChangesAsync();
    }


    public static async Task UpdateProductImagesAsync(ApplicationDbContext context)
    {
        var products = await context.Products.ToListAsync();

        foreach (var product in products)
        {
            switch (product.ProductName)
            {
                // EARRINGS
                case "Golden Pearl Earrings":
                    product.ImageUrl = "/images/earrings.jpg";
                    break;

                case "Classic Hoop Earrings":
                    product.ImageUrl = "/images/earrings/pearl.jpg";
                    break;

                case "Diamond Stud Earrings":
                    product.ImageUrl = "/images/earrings/diamond.jpg";
                    break;

                case "Floral Drop Earrings":
                    product.ImageUrl = "/images/earrings/floral.jpg";
                    break;

                case "Butterfly Earrings":
                    product.ImageUrl = "/images/earrings/butterfly.jpg";
                    break;

                // NECKLACES
                case "Classic Silver Necklace":
                    product.ImageUrl = "/images/necklace.jpg";
                    break;

                case "Golden Chain Necklace":
                    product.ImageUrl = "/images/necklace/chain.jpg";
                    break;

                case "Pearl Necklace":
                    product.ImageUrl = "/images/necklace/pearl.jpg";
                    break;

                case "Layered Necklace":
                    product.ImageUrl = "/images/necklace/layer.jpg";
                    break;

                case "Heart Necklace":
                    product.ImageUrl = "/images/necklace/heart.jpg";
                    break;

                // RINGS
                case "Rose Gold Ring":
                    product.ImageUrl = "/images/ring.jpg";
                    break;

                case "Solitaire Ring":
                    product.ImageUrl = "/images/ring/solitaire.jpg";
                    break;

                case "Pearl Ring":
                    product.ImageUrl = "/images/ring/pearl.jpg";
                    break;

                case "Heart Design Ring":
                    product.ImageUrl = "/images/ring/heart.jpg";
                    break;

                // BRACELETS
                case "Crystal Charm Bracelet":
                    product.ImageUrl = "/images/bracelet.jpg";
                    break;

                case "Tennis Bracelet":
                    product.ImageUrl = "/images/bracelet/tennis.jpg";
                    break;

                case "Golden Chain Bracelet":
                    product.ImageUrl = "/images/bracelet/chain.jpg";
                    break;

                case "Heart Charm Bracelet":
                    product.ImageUrl = "/images/bracelet/heart.jpg";
                    break;

                case "Pearl Charm Bracelet":
                    product.ImageUrl = "/images/bracelet/pearl.jpg";
                    break;
                // PENDANTS
                case "Heart Shape Pendant":
                    product.ImageUrl = "/images/pendant.jpg";
                    break;

                case "Star Pendant":
                    product.ImageUrl = "/images/pendant/star.jpg";
                    break;

                case "Moon Pendant":
                    product.ImageUrl = "/images/pendant/moon.jpg";
                    break;
                case "Butterfly Pendant":
                    product.ImageUrl = "/images/pendant/butterfly.jpg";
                    break;
                case "Pearl Pendant":
                    product.ImageUrl = "/images/pendant/pearl.jpg";
                    break;

            }
        }

        await context.SaveChangesAsync();
    }

    public static async Task CreateAdminAsync(ApplicationDbContext context)
    {
        // Check whether an admin already exists
        bool adminExists = await context.Users
            .AnyAsync(u => u.Role == "Admin");

        if (adminExists)
        {
            return;
        }

        var admin = new User
        {
            FullName = "Dazzle Admin",
            Email = "admin@dazzlejewels.com",
            PhoneNumber = "9800000000",
            Address = "Kathmandu",
            Role = "Admin"
        };

        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

        admin.PasswordHash = passwordHasher.HashPassword(
            admin, "DazzleAdmin@123");

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }



}

