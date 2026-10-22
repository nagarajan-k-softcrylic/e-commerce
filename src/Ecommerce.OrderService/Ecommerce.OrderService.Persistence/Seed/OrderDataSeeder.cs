using Ecommerce.OrderService.Domain.Entities;
using Ecommerce.OrderService.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Persistence.Seed;

public static class OrderDataSeeder
{
    public static async Task SeedAsync(OrderDbContext context, ILogger logger)
    {
        if (await context.Orders.AnyAsync())
            return;

        logger.LogInformation("Seeding initial Order Service sample data...");

        var customerId = "customer@ecommerce.com";

        var order1 = new Order(Guid.Parse("11111111-1111-1111-1111-111111111111"), "ORD-20261001-101", customerId);
        order1.AddItem(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Dell XPS 15 Laptop", 1, 1899.99m);
        order1.Confirm();

        var order2 = new Order(Guid.Parse("22222222-2222-2222-2222-222222222222"), "ORD-20261002-102", customerId);
        order2.AddItem(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Sony WH-1000XM5 Headphones", 2, 349.99m);
        order2.Confirm();

        var order3 = new Order(Guid.Parse("33333333-3333-3333-3333-333333333333"), "ORD-20261003-103", customerId);
        order3.AddItem(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), "LG 27GP850 UltraGear Gaming Monitor", 1, 399.99m);
        // Remains Pending for demonstration

        var order4 = new Order(Guid.Parse("44444444-4444-4444-4444-444444444444"), "ORD-20261004-104", customerId);
        order4.AddItem(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Apple MacBook Pro M3", 1, 2499.00m);
        order4.Fail("Insufficient inventory stock: Requested 1 but 0 available.");

        await context.Orders.AddRangeAsync(order1, order2, order3, order4);
        await context.SaveChangesAsync();

        logger.LogInformation("Order Service sample data seeded successfully.");
    }
}

