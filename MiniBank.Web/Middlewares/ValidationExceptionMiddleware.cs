using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MiniBank.Core.Exceptions;

namespace MiniBank.Web.Middlewares
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (ValidationException userFriendlyException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { userFriendlyException.Message });
            }
        }
    }
}