namespace QLDEducationalWeatherDataAPI.Models.WeatherDTOs
{
    /// <summary>
    /// Data transfer object representing filtered values.
    /// </summary>
    public class FilteredValueDTO
    {
        /// <summary>
        /// Gets and Sets the name of the device associated with the weather data.
        /// </summary>
        public string? DeviceName { get; set; }
        /// <summary>
        /// Gets and Sets the timestamp associated with the weather data.
        /// </summary>
        public DateTime? Time {  get; set; }
        /// <summary>
        /// Gets or Sets the Temperature value associated with the weather data.
        /// </summary>
        public double? Temperature { get; set; }
        /// <summary>
        /// Gets or Sets the Atmospheric Pressure value associated with the weather data.
        /// </summary>
        public double? AtmosphericPressure { get; set; }
        /// <summary>
        /// Gets or Sets the Solar Radiation value associated with the weather data.
        /// </summary>
        public double? SolarRadiation { get; set; }
        /// <summary>
        /// Gets or Sets the precipitation value associated with the weather data.
        /// </summary>
        public double? Precipitation { get; set; }
    }
}
