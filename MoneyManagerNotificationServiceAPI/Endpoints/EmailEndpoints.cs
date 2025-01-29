using MediatR;
using Persistence.Commands.EmailCommands;

namespace MoneyManagerNotificationServiceAPI.Endpoints
{
    public static class EmailEndpoints
    {
        public static WebApplication AddEmailEndpoints(this WebApplication app)
        {
            app.MapPost("/send-weekly-reports", async (SendWeeklyReportCommand request, ISender sender) =>
            {
                var result = await sender.Send(request);
                return Results.Ok(result);
            });

            return app;
        }
    }
}