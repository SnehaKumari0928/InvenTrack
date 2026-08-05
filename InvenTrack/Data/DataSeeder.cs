using InvenTrack.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvenTrack.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Suppliers.AnyAsync())
        {
            var suppliers = new List<Supplier>
            {
                new Supplier
                {
                    Name = "Dell Technologies",
                    Email = "sales@dell.com",
                    Phone = "+1-800-999-3355",
                    Address = "Texas, USA",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new Supplier
                {
                    Name = "Logitech",
                    Email = "sales@logitech.com",
                    Phone = "+41-21-863-5511",
                    Address = "Switzerland",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new Supplier
                {
                    Name = "Samsung",
                    Email = "sales@samsung.com",
                    Phone = "+82-2-2255-0114",
                    Address = "South Korea",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Suppliers.AddRangeAsync(suppliers);
            await context.SaveChangesAsync();
        }

       if (!await context.Products.AnyAsync())
        {
            var dell = await context.Suppliers.FirstAsync(s => s.Name == "Dell Technologies");
            var logitech = await context.Suppliers.FirstAsync(s => s.Name == "Logitech");
            var samsung = await context.Suppliers.FirstAsync(s => s.Name == "Samsung");

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Dell Inspiron 15",
                    SKU = "DEL-LAP-001",
                    Price = 65000,
                    StockQuantity = 20,
                    SupplierId = dell.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new Product
                {
                    Name = "Logitech Mouse",
                    SKU = "LOG-MOU-001",
                    Price = 1500,
                    StockQuantity = 100,
                    SupplierId = logitech.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new Product
                {
                    Name = "Samsung SSD 1TB",
                    SKU = "SAM-SSD-001",
                    Price = 6500,
                    StockQuantity = 40,
                    SupplierId = samsung.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}