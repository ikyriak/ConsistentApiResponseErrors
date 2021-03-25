using ConsistentApiResponseErrors.Filters;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi_3_1.DTOs;

namespace WebApi_3_1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register all validators within a particular assembly:
            services.AddControllers(options =>
            {
                // Use CARE model validator to reduce code duplication
                options.Filters.Add<ValidateModelStateAttribute>();
            })
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<WeatherForecastRequestValidator>());

            // Configure the default API behavour by setting up the MVC application to suppress
            // validation filters in order to be handled by the `ConsistentApiResponseErrors.Filters.ValidateModelStateAttribute`
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // CARE for all Unhandled exceptions
            app.UseMiddleware<ConsistentApiResponseErrors.Middlewares.ExceptionHandlerMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
