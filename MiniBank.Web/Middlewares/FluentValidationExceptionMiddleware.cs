using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace MiniBank.Web.Middlewares
{
    public class FluentValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        public FluentValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (ValidationException fluentValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var errors = fluentValidationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                await context.Response.WriteAsJsonAsync(new { Errors = errors });
            }
        }
    }
}