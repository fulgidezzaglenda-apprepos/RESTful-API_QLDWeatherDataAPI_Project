using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QLDEducationalWeatherDataAPI.Models.WeatherDTOs
{
    /// <summary>
    /// Data transfer object representing updated field values.
    /// </summary>
    public class UpdatedFieldValueDTO
    {
        /// <summary>
        /// Gets or Sets the unique identifier for the weather data.
        /// </summary>
        public ObjectId _id { get; set; }
        /// <summary>
        /// Gets the string representation of the unique identifier for the weather data.
        /// </summary>
        public string ObjId => _id.ToString();
        /// <summary>
        /// Gets or Sets the precipitation value associated with the weather data.
        /// </summary>
        [BsonElement("Precipitation mm/h")]
        public double Precipitation { get; set; }
        /// <summary>
        /// Gets or Sets the Latitude value associated with the weather data.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Gets or Sets the Longitude value associated with the weather data.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Gets or Sets the Temperature value associated with the weather data.
        /// </summary>
        [BsonElement("Temperature (°C)")]
        public double Temperature { get; set; }
        /// <summary>
        ///  Gets or Sets the Atmospheric Pressure value associated with the weather data.
        /// </summary>
        [BsonElement("Atmospheric Pressure (kPa)")]
        public double AtmosphericPressure { get; set; }
        /// <summary>
        /// Gets or Sets the Wind Speed value associated with the weather data.
        /// </summary>
        [BsonElement("Max Wind Speed (m/s)")]
        public double WindSpeed { get; set; }
        /// <summary>
        /// Gets or Sets the Solar Radiation value associated with the weather data.
        /// </summary>
        [BsonElement("Solar Radiation (W/m2)")]
        public double SolarRadiation { get; set; }
        /// <summary>
        ///  Gets or Sets the Vapor Pressure value associated with the weather data.
        /// </summary>
        [BsonElement("Vapor Pressure (kPa)")]
        public double VaporPressure { get; set; }
        /// <summary>
        ///  Gets or Sets the Humidity value associated with the weather data.
        /// </summary>
        [BsonElement("Humidity (%)")]
        public double Humidity { get; set; }
        /// <summary>
        /// Gets or Sets the Wind DIrection value associated with the weather data.
        /// </summary>
        [BsonElement("Wind Direction (°)")]
        public double WindDirection { get; set; }
        /// <summary>
        /// Gets or Sets the timestamp that represents the first creation time of the weather data.
        /// </summary>
        public DateTime FirstCreated { get; set; }
        /// <summary>
        /// Gets or Sets the timestamp representing the last creation time of the weather data.
        /// </summary>
        public DateTime LastCreated { get; set; }
    }
}
