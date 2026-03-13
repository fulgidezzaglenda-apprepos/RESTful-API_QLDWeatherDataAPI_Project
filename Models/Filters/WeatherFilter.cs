namespace QLDEducationalWeatherDataAPI.Models.Filters
{
    /// <summary>
    /// Represents a filter for querying weather data.
    /// </summary>
    public class WeatherFilter
    {
        /// <summary>
        /// Gets or Sets the text search query for filtering data.
        /// </summary>
        public string? TextSearch {  get; set; }
        /// <summary>
        /// Gets or Sets the start date for filtering data.
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// Gets or Sets the end date for filtering data.
        /// </summary>
        public DateTime? EndDate { get; set;}
    }
}
