using Ecommerce.InventoryService.Infrastructure.BackgroundServices;
using Ecommerce.Shared.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.InventoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppEventBus(configuration);
        services.AddHostedService<InventoryServiceEventConsumer>();
        return services;
    }
}

