using System.Net;
using System.Text.Json;
using Repository.DTOs;

namespace FUNewsManagementSystem.Constants
{
    public class ErrorHandleMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // tiếp tục pipeline

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized && !context.Response.HasStarted)
                {
                    await WriteError(context, "Token missing or invalid", "401");
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden && !context.Response.HasStarted)
                {
                    await WriteError(context, "Permission denied", "403");
                }
            }
            catch (Exception ex)
            {
                await WriteError(context, ex.Message, "500");
            }
        }

        private static Task WriteError(HttpContext context, string message, string statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = int.Parse(statusCode);

            var response = APIResponse<object>.Fail(message, statusCode);
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
