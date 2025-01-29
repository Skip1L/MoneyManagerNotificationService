using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Persistence.Authentication.Helpers;

namespace Persistence.Authentication
{
    public class ApiKeyEndpointsFilter : IEndpointFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyEndpointsFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var expectedApiKey))
            {
                return new UnauthorizedHttpObjectResult("API Key missing");
            }

            var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);

            if (apiKey == null || !apiKey.Equals(expectedApiKey))
            {
                return new UnauthorizedHttpObjectResult("Invalid API Key");
            }

            return await next(context);
        }
    }
}
