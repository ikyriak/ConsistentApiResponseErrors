using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi_3_1.DTOs;
using WebApi_3_1.Exceptions;

namespace WebApi_3_1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecastResposne> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastResposne
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public ActionResult Post(WeatherForecastRequest weatherForecastRequest)
        {
            // NOTE: The weatherForecastRequest has been validated by the relative WeatherForecastRequestValidator.

            // TIP: It's recommended to create thin API Controllers, containing only the intended
            // application code logic calls. This is just an example to show how the
            // `ConsistentApiResponseErrors` library handle the input-validation errors and exceptions.


            // Generating an Application exception (for the sake of this example):
            // Let's assume that we must have a forecast for each day, and that a forecast already
            // exists for the date 2021-03-21.
            if (weatherForecastRequest.Date == new DateTime(2021, 3, 24))
            {
                throw new EntityExistsException("The requested weather forecast exists.");
            }

            // Generating an Unhandled Exceptions (for the sake of this example):
            if (weatherForecastRequest.Date == new DateTime(2021, 3, 23))
            {
                // Try to create a non-valid date, resulting in an exception:
                weatherForecastRequest.Date = new DateTime(2021, 2, 31);
            }


            // Return the created resource:
            WeatherForecastResposne forecastResposne = new WeatherForecastResposne()
            {
                Id = Guid.NewGuid(),
                Date = weatherForecastRequest.Date,
                Summary = weatherForecastRequest.Summary,
                TemperatureC = weatherForecastRequest.TemperatureC
            };
            return Ok(forecastResposne);
        }
    }
}
