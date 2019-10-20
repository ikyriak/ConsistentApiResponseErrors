using System;
using System.Threading.Tasks;
using ConsistentApiResponseErrors.ConsistentErrors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ConsistentApiResponseErrors.Middlewares
{
    /// <summary>
    /// Central error/exception handler Middle-ware
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private const string JsonContentType = "application/problem+json";
        private readonly RequestDelegate request;

        private readonly IHostingEnvironment env;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public ExceptionHandlerMiddleware(RequestDelegate next,
            IHostingEnvironment env,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            this.request = next;
            this.env = env;
            this.logger = logger;
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
                await request(context);
            }
            catch (Exception exception)
            {
                string traceID = Guid.NewGuid().ToString();

                ExceptionError apiException = new ExceptionError(exception, traceID);
                if (!env.IsDevelopment())
                {
                    apiException.StackTrace = string.Empty;
                }

                // Log validation errors with this specific traceID:
                logger.LogError(exception, exception.Message + " ({traceId})", traceID);

                //var httpStatusCode = ConfigurateExceptionTypes(exception);
                var httpStatusCode = apiException.StatusCode;

                // Set HTTP status code and content type
                context.Response.StatusCode = httpStatusCode;
                context.Response.ContentType = JsonContentType;

                // Writes / Returns error model to the response (in camelCase format)
                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                    apiException,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
            }
        }

    }

}
