using System.Text;
using System.Text.Json;
using GaNascimento.RabbitMQ.Helpers;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GaNascimento.RabbitMQ;

public abstract class SubscribeService<T, M> : BackgroundService
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly RabbitConfiguration _rabbitConfiguration;

    public SubscribeService(RabbitConfiguration rabbitConfiguration)
    {
        _rabbitConfiguration = rabbitConfiguration;
        _connectionFactory = new ConnectionFactory { Uri = new Uri(rabbitConfiguration.Host ?? throw new InvalidOperationException("RabbitMQ host not found!")) };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        var exchangeName = this.GetExchangeName();
        var queueName = this.GetQueueName(channel);
        var consumer = new EventingBasicConsumer(channel);

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
        channel.QueueBind(queueName, exchangeName, "");
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var messageBody = JsonSerializer.Deserialize<M>(message);

            await this.Consume(messageBody);
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    protected abstract Task Consume(M? messageBody);

    private string GetQueueName(IModel channel)
    {
        PubSubConfiguration? pubSubConfiguration = null;
        _rabbitConfiguration.RegisterPubSub?.TryGetValue(nameof(T), out pubSubConfiguration);

        if (pubSubConfiguration == null)
            throw new InvalidOperationException("Subscribe not registered!");

        if (string.IsNullOrEmpty(pubSubConfiguration.Queue))
            return channel.QueueDeclare().QueueName;

        channel.QueueDeclare(queue: pubSubConfiguration.Queue, durable: true, exclusive: false, autoDelete: false);

        return pubSubConfiguration.Queue;
    }

    private string GetExchangeName()
    {
        PubSubConfiguration? pubSubConfiguration = null;
        _rabbitConfiguration.RegisterPubSub?.TryGetValue(nameof(T), out pubSubConfiguration);

        if (pubSubConfiguration == null)
            throw new InvalidOperationException("Subscribe not registered!");

        return pubSubConfiguration.Exchange;
    }
}