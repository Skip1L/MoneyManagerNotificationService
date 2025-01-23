using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Windows.Input;
using Application.Commands;
using Application.Common;
using Application.Handlers;
using Application.Interfaces;
using DTOs.NotificationDTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;
using Services.TemplateGenerators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/
builder.Services.AddSingleton<SendWeeklyReportCommandHandler>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddSingleton<ICommandHandler<SendWeeklyReportCommand, bool>, SendWeeklyReportCommandHandler>();
builder.Services.AddTransient(sp =>
{
    var smtpClient = new SmtpClient(Environment.GetEnvironmentVariable("Host"))
    {
        Port = int.TryParse(Environment.GetEnvironmentVariable("Port"), out var result) ? result : 587,
        Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("From"), Environment.GetEnvironmentVariable("Password")),
        EnableSsl = true
    };

    return smtpClient;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/send-weekly-reports", async (ICommandDispatcher dispatcher, [FromBody] List<AnalyticEmailRequestDTO> requests, CancellationToken cancellationToken) =>
{
    var command = new SendWeeklyReportCommand(requests);
    bool allEmailsSent = await dispatcher.Dispatch(command, cancellationToken);

    if (allEmailsSent)
    {
        return Results.Ok("All emails sent successfully.");
    }
    else
    {
        return Results.BadRequest("Some emails failed to send.");
    }
});



app.Run();
