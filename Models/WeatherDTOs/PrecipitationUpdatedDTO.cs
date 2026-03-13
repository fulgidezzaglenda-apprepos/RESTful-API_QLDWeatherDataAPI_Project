using MongoDB.Bson;

namespace QLDEducationalWeatherDataAPI.Models.WeatherDTOs
{
    /// <summary>
    /// Data transfer object representing updated precipitation data.
    /// </summary>
    public class PrecipitationUpdatedDTO
    {
        /// <summary>
        /// Gets or Sets the unique identifier for the weather data.
        /// </summary>
        public ObjectId _id {  get; set; }
        /// <summary>
        /// Gets the string representation of the unique identifier for the weather data.
        /// </summary>
        public string ObjID => _id.ToString();
        /// <summary>
        /// Gets or Sets the precipitation value associated with the weather data.
        /// </summary>
        public double? Precipitation {  get; set; }
    }
}
