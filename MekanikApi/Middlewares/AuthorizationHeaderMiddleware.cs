using MekanikApi.Application.DTOs.Common;
using System.Text.Json;

namespace MekanikApi.Middlewares
{
    public class AuthorizationHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var isAuthorizeRequired = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;

            if (isAuthorizeRequired && !context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var apiResponse = new ApiResponse
                {
                    StatusCode = 401,
                    Message = "Unauthorized to access application. Authorization header missing."
                };

                var result = JsonSerializer.Serialize(apiResponse);
                await context.Response.WriteAsync(result);
            }
            else
            {
                await _next(context);
            }
        }
    }
}