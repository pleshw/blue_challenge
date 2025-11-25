namespace BlueChallenge.Api.Configuration;

public class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string TelemetryQueue { get; set; } = "telemetry";
}
