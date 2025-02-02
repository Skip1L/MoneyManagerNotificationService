using Persistence.Authentication;

namespace MoneyManagerNotificationServiceAPI.Middlewares
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);

            if (string.IsNullOrEmpty(apiKey))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var expectedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API key missing");
                return;
            }

            if (!apiKey.Equals(expectedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            await _next(context);
        }
    }
}
