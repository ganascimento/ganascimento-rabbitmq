namespace GaNascimento.RabbitMQ.Helpers;

public class PubSubConfiguration
{
    public required string Exchange { get; set; }
    public string? Queue { get; set; }
}