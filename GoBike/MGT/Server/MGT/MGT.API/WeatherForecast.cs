using System;

namespace MGT.API
{
    /// <summary>
    /// WeatherForecast
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// TemperatureC
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// TemperatureF
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}