using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace GaNascimento.RabbitMQ;

internal class PublishMessage : IPublishMessage
{
    private readonly ConnectionFactory _connectionFactory;

    public PublishMessage(RabbitConfiguration rabitConfiguration)
    {
        _connectionFactory = new ConnectionFactory { Uri = new Uri(rabitConfiguration.Host ?? throw new InvalidOperationException("RabbitMQ host not found!")) };
    }

    public async Task PublishToExchangeAsync(object body, string exchange)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);

        await Task.Run(() =>
        {
            channel.BasicPublish(exchange, "", null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)));
        });
    }

    public async Task PublishToQueueAsync(object body, string queue, string? exchange = null)
    {


        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);

        var messageToSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body));

        await Task.Run(() => channel.BasicPublish(exchange: exchange ?? "",
                             routingKey: queue,
                             mandatory: true,
                             basicProperties: null,
                             body: messageToSend));
    }
}