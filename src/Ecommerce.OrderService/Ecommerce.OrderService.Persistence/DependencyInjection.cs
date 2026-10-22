using Ecommerce.OrderService.Application.Contracts;
using Ecommerce.OrderService.Persistence.Context;
using Ecommerce.OrderService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.OrderService.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var useInMemory = configuration.GetValue<bool>("Database:UseInMemory", string.IsNullOrWhiteSpace(connectionString));

        if (useInMemory || string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<OrderDbContext>(options =>
                options.UseInMemoryDatabase("Ecommerce_OrdersDb"));
        }
        else
        {
            services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName)));
        }

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}

