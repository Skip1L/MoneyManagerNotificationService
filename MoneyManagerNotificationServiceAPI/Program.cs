using System.Net;
using System.Net.Mail;
using Microsoft.OpenApi.Models;
using MoneyManagerNotificationServiceAPI.Middlewares;
using Persistence.Commands.EmailCommands;
using Persistence.Endpoints;
using Services.Interfaces;
using Services.Mapping;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEmailService, HttpEmailService>();
builder.Services.AddTransient(sp =>
{
    return new SmtpClient(Environment.GetEnvironmentVariable("Host"))
    {
        Port = int.TryParse(Environment.GetEnvironmentVariable("Port"), out var result) ? result : 587,
        Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("From"), Environment.GetEnvironmentVariable("Password")),
        EnableSsl = true
    };
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(SendWeeklyReportCommand).Assembly);
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

app.Run();
