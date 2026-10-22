using System.Reflection;
using Ecommerce.OrderService.Application.EventHandlers;
using Ecommerce.Shared.Bus;
using Ecommerce.Shared.Events;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.OrderService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        // Register integration event handlers
        services.AddScoped<IIntegrationEventHandler<InventoryReservedEvent>, InventoryReservedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<InventoryFailedEvent>, InventoryFailedEventHandler>();

        return services;
    }
}

