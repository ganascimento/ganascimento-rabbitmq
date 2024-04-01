using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GaNascimento.RabbitMQ.Helpers;

public class BusConfigurator
{
    private readonly RabbitConfiguration _rabbitConfiguration;
    private readonly IServiceCollection _services;

    public BusConfigurator(RabbitConfiguration rabbitConfiguration, IServiceCollection services)
    {
        _rabbitConfiguration = rabbitConfiguration;
        _services = services;
    }

    public void Host(string host)
    {
        _rabbitConfiguration.Host = host;
    }

    public void RegisterConsumer<T>(string queueName) where T : class, IHostedService
    {
        if (_rabbitConfiguration.RegisterQueuesConsumer == null)
            _rabbitConfiguration.RegisterQueuesConsumer = new Dictionary<string, string>();

        _rabbitConfiguration.RegisterQueuesConsumer.Add(nameof(T), queueName);

        _services.AddHostedService<T>();
    }

    public void RegisterSubscribe<T>(string exchange, string? queue = null) where T : class, IHostedService
    {
        if (_rabbitConfiguration.RegisterPubSub == null)
            _rabbitConfiguration.RegisterPubSub = new Dictionary<string, PubSubConfiguration>();

        _rabbitConfiguration.RegisterPubSub.Add(nameof(T), new PubSubConfiguration
        {
            Exchange = exchange,
            Queue = queue
        });

        _services.AddHostedService<T>();
    }
}