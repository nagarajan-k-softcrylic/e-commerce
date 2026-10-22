using Ecommerce.InventoryService.Domain.Entities;
using Ecommerce.InventoryService.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.InventoryService.Persistence.Seed;

public static class InventoryDataSeeder
{
    public static async Task SeedAsync(InventoryDbContext context, ILogger logger)
    {
        if (await context.Products.AnyAsync())
            return;

        logger.LogInformation("Seeding initial Inventory Service products...");

        var products = new List<Product>
        {
            new Product(
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                "PROD-LAP-001",
                "Dell XPS 15 Laptop",
                "15.6-inch OLED 3.5K display, Intel Core i9, 32GB RAM, 1TB SSD, NVIDIA RTX 4070",
                1899.99m,
                25
            ),
            new Product(
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                "PROD-AUD-002",
                "Sony WH-1000XM5 Headphones",
                "Industry-leading noise cancelling wireless headphones with 30-hour battery life",
                349.99m,
                50
            ),
            new Product(
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                "PROD-MON-003",
                "LG 27GP850 UltraGear Gaming Monitor",
                "27-inch QHD (2560 x 1440) Nano IPS, 1ms, 165Hz (O/C 180Hz), G-SYNC Compatible",
                399.99m,
                15
            ),
            new Product(
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                "PROD-LAP-004",
                "Apple MacBook Pro M3",
                "14-inch Liquid Retina XDR display, Apple M3 Pro chip, 18GB Unified Memory, 512GB SSD",
                2499.00m,
                0 // Zero stock to test out-of-stock failure flow
            ),
            new Product(
                Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                "PROD-PER-005",
                "Logitech MX Master 3S Mouse",
                "Performance wireless mouse with 8K DPI sensor, Quiet Clicks, and MagSpeed scrolling",
                99.99m,
                80
            ),
            new Product(
                Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                "PROD-PER-006",
                "Keychron K2 Mechanical Keyboard",
                "Wireless 75% layout RGB mechanical keyboard with Gateron G Pro Brown switches",
                89.99m,
                45
            ),
            new Product(
                Guid.Parse("10101010-1010-1010-1010-101010101010"),
                "PROD-STR-007",
                "Samsung 990 Pro 2TB NVMe SSD",
                "PCIe 4.0 M.2 2280 internal gaming solid state drive with up to 7450 MB/s speed",
                179.99m,
                30
            )
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} products.", products.Count);
    }
}

