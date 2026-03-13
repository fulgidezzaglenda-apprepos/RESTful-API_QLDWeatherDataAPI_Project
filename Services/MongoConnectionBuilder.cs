using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QLDEducationalWeatherDataAPI.Settings;

namespace QLDEducationalWeatherDataAPI.Services
{
    /// <summary>
    /// Provides methods to build the MongoDB connection.
    /// </summary>
    public class MongoConnectionBuilder
    {
        /// <summary>
        /// Represents the options for MongoDB connection settings.
        /// </summary>
        private readonly IOptions<MongoConnectionSettings> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConnectionBuilder"/> class with the specified connection settings options.
        /// </summary>
        /// <param name="options"> MongoDB connection settings options. </param>
        public MongoConnectionBuilder(IOptions<MongoConnectionSettings> options)
        {
            _options = options;
        }

        /// <summary>
        /// Sets up a MongoDB connection to the database using the connection string and
        /// database name provided by the MongoConnectionSettings object.
        /// </summary>
        /// <returns> A connection to the specified database in the MongoDB server. </returns>
        public IMongoDatabase GetDatabase()
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            return client.GetDatabase(_options.Value.DatabaseName);
        }

        /// <summary>
        /// Sets up a MongoDB connection to the database using the connection string and
        /// database name provided the method's caller.
        /// </summary>
        /// <param name="connection"> The connection string to MongoDB. </param>
        /// <param name="database"> Name of the database to access in MongoDB. </param>
        /// <returns> A connection to the specified database in the MongoDB server. </returns>
        public IMongoDatabase GetDatabase(string connection, string database)
        {
            var client = new MongoClient(connection);
            return client.GetDatabase(database);
        }
    }
}
