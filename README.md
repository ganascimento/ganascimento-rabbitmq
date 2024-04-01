# GaNascimento.RabbitMQ

## About

This library was developed to help use RabbitMQ in .Net with a simple implementation. To create it, the [RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client/7.0.0-alpha.4) library was used.

## Usage

It has two simple implementations which are described below

### Publish Message To Queue

In application config set:

```c#
builder.Services.AddRabbitMQ(config =>
{
    config.Host("amqp://guest:guest@localhost:5672");

    // Set your consumer type
    // Set the queue name
    config.RegisterConsumer<DefaultAppConsumer>("queue.default");
});
```

To create the consumer:

```c#
public class DefaultAppConsumer : ConsumerService<DefaultAppConsumer, DefaultAppConsumerModel>
{
    public DefaultAppConsumer(RabbitConfiguration rabbitConfiguration) : base(rabbitConfiguration)
    {

    }

    protected override Task Consume(DefaultAppConsumerModel? messageBody)
    {
        throw new NotImplementedException();
    }
}

public class DefaultAppConsumerModel
{
    public required string Name { get; set; }
}
```

To publish message to consumer:

```c#
public class Controller
{
    private readonly PublishMessage _publishMessage;

    public Controller(PublishMessage publishMessage)
    {
        _publishMessage = publishMessage;
    }

    public async Task Sender()
    {
        _publishMessage.PublishToQueueAsync(new DefaultAppConsumerModel
        {
            Name = "Guilherme"
        }, "queue.default");
    }
}
```

### PubSub

In application config set:

```c#
builder.Services.AddRabbitMQ(config =>
{
    config.Host("amqp://guest:guest@localhost:5672");

    // Set your consumer type
    // Set the queue name
    // Queue name is optional
    config.RegisterSubscribe<DefaultAppConsumer>("exchange.default", "queue.default");
});
```

To create the consumer:

```c#
public class DefaultAppConsumer : SubscribeService<DefaultAppConsumer, DefaultAppConsumerModel>
{
    public DefaultAppConsumer(RabbitConfiguration rabbitConfiguration) : base(rabbitConfiguration)
    {

    }

    protected override Task Consume(DefaultAppConsumerModel? messageBody)
    {
        throw new NotImplementedException();
    }
}

public class DefaultAppConsumerModel
{
    public required string Name { get; set; }
}
```

To publish message to exchange:

```c#
public class Controller
{
    private readonly PublishMessage _publishMessage;

    public Controller(PublishMessage publishMessage)
    {
        _publishMessage = publishMessage;
    }

    public async Task Sender()
    {
        _publishMessage.PublishToExchangeAsync(new DefaultAppConsumerModel
        {
            Name = "Guilherme"
        }, "exchange.default");
    }
}
```
