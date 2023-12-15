using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace VirtualPaymentService.Exceptions
{
    /// <summary>
    /// Provides an <see cref="Exception"/> catch all for the request Pipeline.
    /// <para>Should be added after the Correlation middleware in order to capture request path in logs.</para>
    /// </summary>
    public class UnhandledExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public UnhandledExceptionMiddleware(ILogger<UnhandledExceptionMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "An unhandled exception has occured", ex);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Place a generic exception message in the response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpStatusCode"></param>
        /// <returns></returns>
        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode httpStatusCode)
        {
            object result = new
            {
                Code = (int)httpStatusCode,
                Status = "UNKNOWN",
                Message = @"An error occured while processing the request.  Please try again.  If issues continue, contact support."
            };
            var response = JsonSerializer.Serialize(result);
            context.Response.StatusCode = (int)httpStatusCode;
            return context.Response.WriteAsync(response);
        }
    }

    /// <summary>
    /// Helper method to setup the middleware.
    /// </summary>
    public static class UnhandledExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Attach the <see cref="UnhandledExceptionMiddleware"/> to the application.
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureUnhandledExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<UnhandledExceptionMiddleware>();
        }
    }
}
