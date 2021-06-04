using System;

namespace WebApi_5_0.DTOs
{
    /// <summary>
    /// Let's assume that this is the Reponse DTO, which contains the data that we would like to be
    /// sent from the API-Client.
    /// </summary>
    public class WeatherForecastRequest
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }
    }
}
