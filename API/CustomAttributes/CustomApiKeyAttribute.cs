using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.CustomAttributes
{
    [AttributeUsage(validOn: AttributeTargets.Class)]
    public class CustomApiKeyAttribute : Attribute, IAsyncActionFilter
    {
       private const string APIKEYNAME = "ApiKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api Key Not Found"
                };
                return;
            }
 
            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
 
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);
 
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Unauthorized Client"
                };
                return;
            }
 
            await next();
        }
    }
}