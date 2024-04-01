using GaNascimento.RabbitMQ.Helpers;

namespace GaNascimento.RabbitMQ;

public class RabbitConfiguration
{
    public string? Host { get; set; }
    public Dictionary<string, PubSubConfiguration>? RegisterPubSub { get; set; }
    public Dictionary<string, string>? RegisterQueuesConsumer { get; set; }
}