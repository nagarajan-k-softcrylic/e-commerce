using Ecommerce.Shared.Bus.AzureServiceBus;
using Ecommerce.Shared.Bus.InMemory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Shared.Bus;

public static class EventBusExtensions
{
    public static IServiceCollection AddAppEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(AzureServiceBusOptions.SectionName);
        services.Configure<AzureServiceBusOptions>(section);

        var options = section.Get<AzureServiceBusOptions>() ?? new AzureServiceBusOptions();
        var provider = configuration["EventBus:Provider"] ?? "Auto";

        bool useAzure = (provider.Equals("AzureServiceBus", StringComparison.OrdinalIgnoreCase) ||
                         (provider.Equals("Auto", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(options.ConnectionString)));

        if (useAzure)
        {
            services.AddSingleton<IEventBus, AzureServiceBusEventBus>();
        }
        else
        {
            services.AddSingleton<InMemoryEventBus>();
            services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<InMemoryEventBus>());
            services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<InMemoryEventBus>());
        }

        return services;
    }
}

