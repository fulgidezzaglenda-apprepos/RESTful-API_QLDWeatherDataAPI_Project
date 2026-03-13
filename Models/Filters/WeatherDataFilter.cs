using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace QLDEducationalWeatherDataAPI.Models
{
    /// <summary>
    /// Represents a filter for querying weather data.
    /// </summary>
    public class WeatherDataFilter
    {
        /// <summary>
        /// Gets or Sets the device name match for filtering weather data.
        /// </summary>
        [BsonElement("Device Name")]
        public string? DeviceNameMatch { get; set; }
        /// <summary>
        /// Gets or Sets the time match for filtering weather data.
        /// </summary>
        public DateTime? TimeMatch { get; set; }
        /// <summary>
        /// Gets or Sets the Precipitaion match for filtering weather data.
        /// </summary>
        [BsonElement("Precipitation mm/h")]
        public double PrecipitationMatch { get; set; }
        /// <summary>
        /// Gets or Sets the Latitude match for filtering weather data.
        /// </summary>
        public double LatitudeMatch { get; set; }
        /// <summary>
        /// Gets or Sets the Longitude match for filtering weather data.
        /// </summary>
        public double LongitudeMatch { get; set; }
        /// <summary>
        /// Gets or Sets the Temperature match for filtering weather data.
        /// </summary>
        [BsonElement("Temperature (°C)")]
        public double TemperatureMatch { get; set; }
        /// <summary>
        /// Gets or Sets the Atmospheric Pressure match for filtering weather data.
        /// </summary>
        [BsonElement("Atmospheric Pressure (kPa)")]
        public double AtmosphericPressureMatch { get; set; }
        /// <summary>
        /// Gets or Sets the WindSpeed match for filtering weather data.
        /// </summary>
        [BsonElement("Max Wind Speed (m/s)")]
        public double WindSpeedMatch { get; set; }
        /// <summary>
        /// Gets or Sets the SolarRadiation match for filtering weather data.
        /// </summary>
        [BsonElement("Solar Radiation (W/m2)")]
        public double SolarRadiationMatch { get; set; }
        /// <summary>
        /// Gets or Sets the VaporPressure match for filtering weather data.
        /// </summary>
        [BsonElement("Vapor Pressure (kPa)")]
        public double VaporPressureMatch { get; set; }
        /// <summary>
        /// Gets or Sets the Humidity match for filtering weather data.
        /// </summary>
        [BsonElement("Humidity (%)")]
        public double HumidityMatch { get; set; }
        /// <summary>
        /// Gets or Sets the WindDirection match for filtering weather data.
        /// </summary>
        [BsonElement("Wind Direction (°)")]
        public double WindDirectionMatch { get; set; }
    }
}
