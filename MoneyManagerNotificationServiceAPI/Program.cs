using System.Net;
using System.Net.Mail;
using MoneyManagerNotificationServiceAPI.Middlewares;
using Persistence.Commands.EmailCommands;
using Services.Interfaces;
using Services.Services;
using MoneyManagerNotificationServiceAPI.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEmailService, EmailService>();
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionsMiddleware>();

app.UseHttpsRedirection();

app.AddEmailEndpoints();

app.Run();
