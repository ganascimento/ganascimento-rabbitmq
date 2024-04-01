using GaNascimento.RabbitMQ.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GaNascimento.RabbitMQ;

public static class ConfigureInjection
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, Action<BusConfigurator>? config = null)
    {
        services.AddSingleton(new RabbitConfiguration());
        services.AddSingleton<IPublishMessage, PublishMessage>();

        if (config != null)
        {
            var rabbitConfiguration = services.BuildServiceProvider().GetService<RabbitConfiguration>() ??
                                        throw new InvalidOperationException("Instance not found!");

            config.Invoke(new BusConfigurator(rabbitConfiguration, services));
        }

        return services;
    }
}