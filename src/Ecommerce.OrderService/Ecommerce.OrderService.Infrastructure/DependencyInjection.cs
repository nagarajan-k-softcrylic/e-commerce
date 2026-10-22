using Ecommerce.OrderService.Infrastructure.BackgroundServices;
using Ecommerce.Shared.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Event Bus abstraction (Azure Service Bus or InMemory fallback)
        services.AddAppEventBus(configuration);

        // Register background consumer worker
        services.AddHostedService<OrderServiceEventConsumer>();

        return services;
    }
}

