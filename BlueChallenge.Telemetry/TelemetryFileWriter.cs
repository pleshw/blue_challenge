using System.Text;

namespace BlueChallenge.Telemetry;

public class TelemetryFileWriter
{
    private readonly string _basePath;

    public TelemetryFileWriter(IConfiguration configuration)
    {
        _basePath = configuration["Telemetry:StoragePath"]
                    ?? Path.Combine(AppContext.BaseDirectory, "telemetry_logs");
    }

    public async Task WriteAsync(string userIdentifier, string payload, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userIdentifier))
        {
            userIdentifier = "unknown";
        }

        Directory.CreateDirectory(_basePath);

        var safeUserName = SanitizeFileName(userIdentifier);
        var filePath = Path.Combine(_basePath, $"{safeUserName}.log");
        var line = $"[{DateTimeOffset.UtcNow:u}] {payload}{Environment.NewLine}";

        await File.AppendAllTextAsync(filePath, line, Encoding.UTF8, cancellationToken);
    }

    private static string SanitizeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(value.Length);

        foreach (var ch in value)
        {
            builder.Append(invalidChars.Contains(ch) ? '_' : ch);
        }

        return builder.ToString();
    }
}
