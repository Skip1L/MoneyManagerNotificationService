using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Persistence.Authentication
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeySectionName, out var expectedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key missing");
            }
            
            var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);

            if (apiKey == null || !apiKey.Equals(expectedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Invalid API Key");
            }
        }
    }
}
