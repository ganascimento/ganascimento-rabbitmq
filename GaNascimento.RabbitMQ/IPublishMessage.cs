namespace GaNascimento.RabbitMQ;

public interface IPublishMessage
{
    /// <summary>
    /// Publish message to exchange
    /// </summary>
    /// <param name="body">Message to send</param>
    /// <param name="exchange">Exchange name</param>
    /// <returns>Return task void</returns>
    Task PublishToExchangeAsync(object body, string exchange);

    /// <summary>
    /// Publish message to queue
    /// </summary>
    /// <param name="body">Message to send</param>
    /// <param name="queue">Queue name</param>
    /// <param name="exchange">Exchange name</param>
    /// <returns>Return task void</returns>
    Task PublishToQueueAsync(object body, string queue, string? exchange = null);
}