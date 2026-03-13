namespace QLDEducationalWeatherDataAPI.Settings
{
    /// <summary>
    /// Represents the settings for MongoDB connection
    /// </summary>
    public class MongoConnectionSettings
    {
        /// <summary>
        /// Gets or Sets the MongoDB connection string.
        /// </summary>
        public string ConnectionString { get; set; } //= string.Empty;
        /// <summary>
        /// Gets or Sets the name of the MongoDB database.
        /// </summary>
        public string DatabaseName {  get; set; } //= string.Empty;
    }
}
