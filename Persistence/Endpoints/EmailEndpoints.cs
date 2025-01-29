using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Persistence.Authentication;
using Persistence.Commands.EmailCommands;

namespace Persistence.Endpoints
{
    public static class EmailEndpoints
    {
        public static WebApplication AddEmailEndpoints(this WebApplication app)
        {
            app.MapPost("/send-weekly-reports", async (SendWeeklyReportCommand request, ISender sender) =>
            {
                var result = await sender.Send(request);
                return Results.Ok(result);
            }).AddEndpointFilter<ApiKeyEndpointsFilter>();

            return app;
        }
    }
}