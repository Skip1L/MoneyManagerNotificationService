using Confluent.Kafka;
using DTOs.Exchanges;
using MassTransit;
using Microsoft.OpenApi.Models;
using MoneyManagerNotificationServiceAPI.Configs;
using MoneyManagerNotificationServiceAPI.Helpers;
using MoneyManagerNotificationServiceAPI.Middlewares;
using Persistence.Commands.EmailCommands;
using Persistence.Endpoints;
using Serilog;
using Services.Configs;
using Services.Interfaces;
using Services.Mapping;
using Services.Services;
using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables("NOTIFICATION_");

builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<MessageBrokerConfig>(builder.Configuration.GetSection("MessageBroker"));
builder.Services.Configure<ApiKeyConfig>(builder.Configuration.GetSection("ApiKey"));

var smtpConfig = builder.Configuration.GetSection("Smtp").Get<SmtpConfig>();
var brokerConfig = builder.Configuration.GetSection("MessageBroker").Get<MessageBrokerConfig>();
var apiKeyConfig = builder.Configuration.GetSection("ApiKey").Get<ApiKeyConfig>();

Console.WriteLine($"SMTP Host: {smtpConfig?.Host}");
Console.WriteLine($"Kafka Host: {brokerConfig?.KafkaHost}");
Console.WriteLine($"RabbitMQ Host: {brokerConfig?.RabbitMQHost}");

builder.Services.AddSingleton(smtpConfig);
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient(sp =>
{
    return new SmtpClient(smtpConfig.Host)
    {
        Port = smtpConfig.Port,
        Credentials = new NetworkCredential(smtpConfig.From, smtpConfig.Password),
        EnableSsl = true
    };
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(SendWeeklyReportCommand).Assembly);
});

await KafkaTopics.EnsureExistsAsync(brokerConfig.KafkaHost, "analytic-email-created");

builder.Services.AddMassTransit(m =>
{
    m.SetKebabCaseEndpointNameFormatter();
    m.AddConsumer<AnalyticEmailCreatedConsumer>();

    m.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(brokerConfig.RabbitMQHost, "/", c =>
        {
            c.Username(brokerConfig.RabbitMQUsername);
            c.Password(brokerConfig.RabbitMQPassword);
        });
        cfg.ReceiveEndpoint("analytic-email-created", e =>
        {
            e.ConfigureConsumer<AnalyticEmailCreatedConsumer>(ctx);
        });
    });

    m.AddRider(rider =>
    {
        rider.AddConsumer<AnalyticEmailCreatedConsumer>();

        rider.UsingKafka((ctx, k) =>
        {
            k.Host(brokerConfig.KafkaHost);

            k.TopicEndpoint<AnalyticEmailCreated>(
                topicName: "analytic-email-created",
                groupId: "notification-service",
                e =>
                {
                    e.AutoOffsetReset = AutoOffsetReset.Earliest;
                    e.ConfigureConsumer<AnalyticEmailCreatedConsumer>(ctx);
                });
        });
    });
});

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddAutoMapper(typeof(GrpcMappingProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(conf => {
    conf.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "The API key to access the API",
        Type = SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    conf.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.Debug()
    .CreateLogger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<GrpcEmailService>();
app.MapGrpcReflectionService();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseMiddleware<ExceptionsMiddleware>();

app.UseHttpsRedirection();

app.AddEmailEndpoints();

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "Notification Service",
    timestamp = DateTime.UtcNow
}));

app.Run();
