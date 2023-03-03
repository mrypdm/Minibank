using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace MiniBank.Web.Middlewares
{
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        private class TokenPayload
        {
            public long exp { get; init; }
        }

        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private string DecodeBase64(string base64String)
        {
            var rawData = WebEncoders.Base64UrlDecode(base64String);
            var originalString = Encoding.UTF8.GetString(rawData);
            return originalString;
        }

        private DateTime? GetExpTime(string token)
        {
            try
            {
                var payloadString = DecodeBase64(token.Split('.')[1]);

                var payload = JsonSerializer.Deserialize<TokenPayload>(payloadString);

                var exp = DateTimeOffset.FromUnixTimeSeconds(payload!.exp).LocalDateTime;

                return exp;
            }
            catch
            {
                return null;
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault(entry => entry.Contains("Bearer"));

            var exp = GetExpTime(token);

            if (exp.HasValue && DateTime.Now < exp)
            {
                await _next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
        }
    }
}