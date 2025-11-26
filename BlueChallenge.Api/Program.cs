using BlueChallenge.Api.Configuration;
using BlueChallenge.Api.Data;
using BlueChallenge.Api.Repository;
using BlueChallenge.Api.Service;
using BlueChallenge.Api.Service.Auth;
using BlueChallenge.Api.Service.Telemetry;
using BlueChallenge.Api.Validation;
using BlueChallenge.Telemetry;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<UserModelValidator>();
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<ITelemetryProducer, RabbitMqTelemetryProducer>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddScoped<AuthenticationService>();

var telemetryEnabled = !builder.Environment.IsEnvironment("Testing") &&
    builder.Configuration.GetValue("Telemetry:EnableConsumer", true);

if (telemetryEnabled)
{
    builder.Services.AddSingleton<TelemetryFileWriter>();
    builder.Services.AddHostedService<RabbitMqConsumer>();
}

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration section is missing.");

var signingKey = Encoding.UTF8.GetBytes(jwtOptions.SigningKey ?? string.Empty);
if (signingKey.Length == 0)
{
    throw new InvalidOperationException("JWT signing key is not configured.");
}

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKey),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
