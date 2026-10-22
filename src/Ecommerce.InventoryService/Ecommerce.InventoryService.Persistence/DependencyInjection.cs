using Ecommerce.InventoryService.Application.Contracts;
using Ecommerce.InventoryService.Persistence.Context;
using Ecommerce.InventoryService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.InventoryService.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var useInMemory = configuration.GetValue<bool>("Database:UseInMemory", string.IsNullOrWhiteSpace(connectionString));

        if (useInMemory || string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseInMemoryDatabase("Ecommerce_InventoryDb"));
        }
        else
        {
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName)));
        }

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}

