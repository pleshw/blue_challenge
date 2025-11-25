using BlueChallenge.Api.Configuration;
using BlueChallenge.Api.Data;
using BlueChallenge.Api.Repository;
using BlueChallenge.Api.Service;
using BlueChallenge.Api.Service.Telemetry;
using BlueChallenge.Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<UserModelValidator>();
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<ITelemetryProducer, RabbitMqTelemetryProducer>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ScheduleService>();

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<BlueChallengeDbContext>(options =>
        options.UseInMemoryDatabase("BlueChallengeTests"));
}
else
{
    var dbConnection = builder.Configuration.GetValue<string>("Database:ConnectionString");
    builder.Services.AddDbContext<BlueChallengeDbContext>(options =>
        options.UseSqlite(dbConnection));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
