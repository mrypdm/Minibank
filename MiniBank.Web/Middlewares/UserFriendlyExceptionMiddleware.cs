using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MiniBank.Core.Exceptions;

namespace MiniBank.Web.Middlewares
{
    public class UserFriendlyExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        public UserFriendlyExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (UserFriendlyException userFriendlyException)
            {
                await context.Response.WriteAsJsonAsync(new { userFriendlyException.Message });
            }
        }
    }
}