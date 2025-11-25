namespace BlueChallenge.Api.Service.Telemetry;

public interface ITelemetryProducer
{
    Task PublishAsync<TPayload>(string eventType, TPayload payload, CancellationToken cancellationToken = default);
}
