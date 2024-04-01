using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GaNascimento.RabbitMQ;

/// <summary>
/// Service to configure consumer
/// </summary>
/// <typeparam name="T">Consumer class</typeparam>
/// <typeparam name="M">Class with model received on message</typeparam>
public abstract class ConsumerService<T, M> : BackgroundService
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly RabbitConfiguration _rabbitConfiguration;

    public ConsumerService(RabbitConfiguration rabbitConfiguration)
    {
        _rabbitConfiguration = rabbitConfiguration;
        _connectionFactory = new ConnectionFactory { Uri = new Uri(rabbitConfiguration.Host ?? throw new InvalidOperationException("RabbitMQ host not found!")) };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        var queueName = this.GetQueueName();
        var consumer = new EventingBasicConsumer(channel);

        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
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

    private string GetQueueName()
    {
        var queueName = "";
        _rabbitConfiguration.RegisterQueuesConsumer?.TryGetValue(nameof(T), out queueName);

        if (string.IsNullOrEmpty(queueName))
            throw new InvalidOperationException("Queue not registered!");

        return queueName;
    }
}