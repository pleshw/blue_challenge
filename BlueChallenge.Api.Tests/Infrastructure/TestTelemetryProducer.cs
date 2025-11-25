using BlueChallenge.Api.Service.Telemetry;

namespace BlueChallenge.Api.Tests.Infrastructure;

public class TestTelemetryProducer : ITelemetryProducer
{
    private readonly List<(string EventType, object Payload)> _events = new();

    public IReadOnlyList<(string EventType, object Payload)> Events => _events;

    public Task PublishAsync<TPayload>(string eventType, TPayload payload, CancellationToken cancellationToken = default)
    {
        object storedPayload = payload == null ? string.Empty : payload;
        _events.Add((eventType, storedPayload));
        return Task.CompletedTask;
    }

    public void Clear() => _events.Clear();
}
