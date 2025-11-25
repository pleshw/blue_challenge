using System.Text;
using System.Text.Json;
using BlueChallenge.Api.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BlueChallenge.Api.Service.Telemetry;

public class RabbitMqTelemetryProducer : ITelemetryProducer
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqTelemetryProducer> _logger;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ConnectionFactory _factory;

    public RabbitMqTelemetryProducer(IOptions<RabbitMqOptions> options, ILogger<RabbitMqTelemetryProducer> logger)
    {
        _options = options.Value;
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        _factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
            UserName = _options.User,
            Password = _options.Password
        };
    }

    public async Task PublishAsync<TPayload>(string eventType, TPayload payload, CancellationToken cancellationToken = default)
    {
        var envelope = new TelemetryEnvelope<TPayload>(eventType, DateTimeOffset.UtcNow, payload);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(envelope, _serializerOptions));

        await using var connection = await _factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: false,
                publisherConfirmationTrackingEnabled: false,
                outstandingPublisherConfirmationsRateLimiter: null,
                consumerDispatchConcurrency: null),
            cancellationToken);

        await channel.QueueDeclareAsync(
            queue: _options.TelemetryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            noWait: false,
            cancellationToken: cancellationToken);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _options.TelemetryQueue,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Telemetry event {EventType} published", eventType);
    }

    private sealed record TelemetryEnvelope<TPayload>(string EventType, DateTimeOffset Timestamp, TPayload Payload);
}
