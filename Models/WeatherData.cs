using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace QLDEducationalWeatherDataAPI.Models
{
    /// <summary>
    /// Represents weather data that includes all the double properties, string, datetime, and objectId.
    /// </summary>
    public class WeatherData
    {
        /// <summary>
        /// Gets or Sets the unique identifier for the weather data.
        /// </summary>
        [JsonIgnore]
        [BsonId]
        public ObjectId _id {  get; set; }
        /// <summary>
        /// Gets the string representation of the unique identifier for the weather data.
        /// </summary>
        public string ObjId => _id.ToString();
        /// <summary>
        /// Gets and Sets the name of the device associated with the weather data.
        /// </summary>
        [BsonElement("Device Name")]
        public string? DeviceName { get; set; }
        /// <summary>
        /// Gets and Sets the timestamp associated with the weather data.
        /// </summary>
        public DateTime? Time { get; set; }
        /// <summary>
        /// Gets or Sets the precipitation value associated with the weather data.
        /// </summary>
        [BsonElement("Precipitation mm/h")]
        public double? Precipitation { get; set; }
        /// <summary>
        /// Gets or Sets the Latitude value associated with the weather data.
        /// </summary>
        [BsonElement("Latitude")]
        public double? Latitude { get; set; } = 1;
        /// <summary>
        /// Gets or Sets the LOngitude value associated with the weather data.
        /// </summary>
        [BsonElement("Longitude")]
        public double? Longitude { get; set; } = 2;
        /// <summary>
        /// Gets or Sets the Temperature value associated with the weather data.
        /// </summary>
        [BsonElement("Temperature (°C)")]
        public double? Temperature { get; set; } = 3;
        /// <summary>
        /// Gets or Sets the Atmospheric Pressure value associated with the weather data.
        /// </summary>
        [BsonElement("Atmospheric Pressure (kPa)")]
        public double? AtmosphericPressure { get; set; } = 4;
        /// <summary>
        /// Gets or Sets the Wind Speed value associated with the weather data.
        /// </summary>
        [BsonElement("Max Wind Speed (m/s)")]
        public double? WindSpeed { get; set; } = 5;
        /// <summary>
        /// Gets or Sets the Solar Radiation value associated with the weather data.
        /// </summary>
        [BsonElement("Solar Radiation (W/m2)")]
        public double? SolarRadiation { get; set; } = 6;
        /// <summary>
        /// Gets or Sets the Vapor Pressure value associated with the weather data.
        /// </summary>
        [BsonElement("Vapor Pressure (kPa)")]
        public double? VaporPressure { get; set; } = 7;
        /// <summary>
        /// Gets or Sets the Humidity value associated with the weather data.
        /// </summary>
        [BsonElement("Humidity (%)")]
        public double? Humidity { get; set; } = 8;
        /// <summary>
        /// Gets or Sets the Wind DIrection value associated with the weather data.
        /// </summary>
        [BsonElement("Wind Direction (°)")]
        public double? WindDirection { get; set; } = 9;
        /// <summary>
        /// Gets or Sets the DateTime of the creation time for the weather data.
        /// </summary>
        public DateTime Created { get; set; }
    }
}
