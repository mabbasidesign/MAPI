using System.Net;
using System.Text.Json;

namespace MAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorResponse = new
                {
                    message = "An unexpected error occurred.",
                    detail = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }
}
