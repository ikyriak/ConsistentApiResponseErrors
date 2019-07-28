using System;
using System.Threading.Tasks;
using ConsistentApiResponseErrors.ConsistentErrors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ConsistentApiResponseErrors.Middlewares
{
    /// <summary>
    /// Central error/exception handler Middleware
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private const string JsonContentType = "application/problem+json";
        private readonly RequestDelegate request;

        private readonly IHostingEnvironment env;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public ExceptionHandlerMiddleware(RequestDelegate next, IHostingEnvironment env)
        {
            this.request = next;
            this.env = env;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task Invoke(HttpContext context) => this.InvokeAsync(context);

        async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.request(context);
            }
            catch (Exception exception)
            {

                // TODO: Logging validation errors with this specific traceID:
                string traceID = Guid.NewGuid().ToString();

                ExceptionError apiException = new ExceptionError(exception, traceID);
                if (!env.IsDevelopment())
                {
                    apiException.StackTrace = string.Empty;
                }

                //var httpStatusCode = ConfigurateExceptionTypes(exception);
                var httpStatusCode = apiException.StatusCode;

                // Set http status code and content type
                context.Response.StatusCode = httpStatusCode;
                context.Response.ContentType = JsonContentType;

                // Writes / Returns error model to the response
                await context.Response.WriteAsync(JsonConvert.SerializeObject(apiException));
            }
        }

    }

}
