namespace QLDEducationalWeatherDataAPI.Models.WeatherDTOs
{
    /// <summary>
    /// Data transfer object representing maximum temperature data.
    /// </summary>
    public class MaxTempDTO
    {
        /// <summary>
        /// Gets and Sets the name of the device associated with the weather data.
        /// </summary>
        public string? DeviceName { get; set; }
        /// <summary>
        /// Gets and Sets the timestamp associated with the weather data.
        /// </summary>
        public DateTime? Time { get; set; }
        /// <summary>
        /// Gets or Sets the Temperature value associated with the weather data.
        /// </summary>
        public double? Temperature { get; set; }

    }
}
