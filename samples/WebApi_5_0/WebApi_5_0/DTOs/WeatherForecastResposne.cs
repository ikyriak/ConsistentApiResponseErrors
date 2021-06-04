using System;

namespace WebApi_5_0.DTOs
{
    /// <summary>
    /// Let's assume that this is the Reponse DTO, which contains the dta that we would like to
    /// return to the API-Client for each weather forecast.
    /// </summary>
    public class WeatherForecastResposne
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
