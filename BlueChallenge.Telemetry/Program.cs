using BlueChallenge.Telemetry;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<TelemetryFileWriter>();
builder.Services.AddHostedService<RabbitMqConsumer>();

var host = builder.Build();
host.Run();
