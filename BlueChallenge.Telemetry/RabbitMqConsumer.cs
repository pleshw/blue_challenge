using System.Globalization;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BlueChallenge.Telemetry;

public class RabbitMqConsumer : BackgroundService
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly TelemetryFileWriter _fileWriter;

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IConfiguration configuration, TelemetryFileWriter fileWriter)
    {
        _logger = logger;
        _configuration = configuration;
        _fileWriter = fileWriter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_configuration == null)
        {
            _logger.LogError("RabbitMQ configuration is missing.");
            return;
        }

        var queueName = _configuration["RabbitMq:TelemetryQueue"] ?? "telemetry";

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:Host"] ?? "localhost",
            Port = _configuration.GetValue("RabbitMq:Port", 5672),
            UserName = _configuration["RabbitMq:User"] ?? ConnectionFactory.DefaultUser,
            Password = _configuration["RabbitMq:Password"] ?? ConnectionFactory.DefaultPass,
            VirtualHost = _configuration["RabbitMq:VirtualHost"] ?? ConnectionFactory.DefaultVHost
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            IConnection? connection = null;
            IChannel? channel = null;

            try
            {
                connection = await factory.CreateConnectionAsync(stoppingToken);
                channel = await connection.CreateChannelAsync(
                    new CreateChannelOptions(
                        publisherConfirmationsEnabled: false,
                        publisherConfirmationTrackingEnabled: false,
                        outstandingPublisherConfirmationsRateLimiter: null,
                        consumerDispatchConcurrency: null),
                    stoppingToken);

                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    noWait: false,
                    cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.Span);
                    var (user, eventType) = ExtractRoutingInfo(message);
                    await _fileWriter.WriteAsync(user, message, stoppingToken);
                    _logger.LogInformation("Received telemetry event {EventType} for {User}: {Message}", eventType, user, message);
                };

                await channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: true,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ consumer disconnected. Retrying in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            finally
            {
                if (channel is not null)
                {
                    try
                    {
                        await channel.CloseAsync(Constants.ReplySuccess, "Shutting down", false, CancellationToken.None);
                    }
                    catch (Exception closeEx)
                    {
                        _logger.LogDebug(closeEx, "Failed to close RabbitMQ channel cleanly");
                    }
                    channel.Dispose();
                }

                if (connection is not null)
                {
                    try
                    {
                        await connection.CloseAsync(CancellationToken.None);
                    }
                    catch (Exception closeEx)
                    {
                        _logger.LogDebug(closeEx, "Failed to close RabbitMQ connection cleanly");
                    }
                    connection.Dispose();
                }
            }
        }
    }

    private static (string userIdentifier, string eventType) ExtractRoutingInfo(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return ("unknown", "unknown");
        }

        try
        {
            using var document = JsonDocument.Parse(message);
            var root = document.RootElement;

            var eventType = ReadPropertyAsString(root, "EventType") ?? "unknown";
            var payload = root.TryGetProperty("Payload", out var payloadElement) ? payloadElement : root;
            var user = ResolveUserIdentifier(eventType, payload);

            if (user == "unknown")
            {
                user = ExtractLegacyUserIdentifier(message);
            }

            return (user, eventType);
        }
        catch (JsonException)
        {
            return (ExtractLegacyUserIdentifier(message), "unknown");
        }
    }

    private static string ResolveUserIdentifier(string eventType, JsonElement payload)
    {

        // Map each known event type to the most reliable identifier in its payload so logs stay grouped per entity.
        string? identifier = eventType switch
        {
            "UserCreated" or "UserUpdated" => ReadPropertyAsString(payload, "Id")
                                                ?? ReadPropertyAsString(payload, "UserId")
                                                ?? ReadPropertyAsString(payload, "Email"),
            "UserDeleted" => ReadPropertyAsString(payload, "UserId"),
            "ScheduleCreated" or "ScheduleUpdated" => ReadPropertyAsString(payload, "UserId")
                                                         ?? ReadPropertyAsString(payload, "Id"),
            "ScheduleDeleted" => ReadPropertyAsString(payload, "ScheduleId"),
            "ScheduleServiceError" => "schedule-errors",
            _ => ReadPropertyAsString(payload, "UserId")
                 ?? ReadPropertyAsString(payload, "Id")
                 ?? ReadPropertyAsString(payload, "ScheduleId")
        };

        return string.IsNullOrWhiteSpace(identifier) ? "unknown" : identifier;
    }

    private static string ExtractLegacyUserIdentifier(string message)
    {
        try
        {
            using var document = JsonDocument.Parse(message);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (ReadPropertyAsString(root, "userId") is { } legacyUser)
                {
                    return legacyUser;
                }
            }
        }
        catch (JsonException)
        {
            // ignored
        }

        var separatorIndex = message.IndexOf(':');
        if (separatorIndex > 0)
        {
            return message[..separatorIndex].Trim();
        }

        return "unknown";
    }

    private static string? ReadPropertyAsString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return null;
        }

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString(),
            JsonValueKind.Number => property.TryGetInt64(out var number)
                ? number.ToString(CultureInfo.InvariantCulture)
                : property.ToString(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            _ => property.ValueKind == JsonValueKind.Null ? null : property.ToString()
        };
    }
}
