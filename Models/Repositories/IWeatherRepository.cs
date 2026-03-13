using QLDEducationalWeatherDataAPI.Models.WeatherDTOs;

namespace QLDEducationalWeatherDataAPI.Models.Repositories
{
    /// <summary>
    /// Represents a repository interface for managing the weather data.
    /// </summary>
    public interface IWeatherRepository : IGenericRepository<WeatherData>
    {
        /// <summary>
        /// Creates multiple weather data records.
        /// </summary>
        /// <param name="dataList"> The list of weather data that will be created. </param>
        void CreateMany(List<WeatherData> dataList);
        /// <summary>
        /// Retrieves the maximum precipitation value for a given number of days and device name.
        /// </summary>
        /// <param name="days"> Calculates the number of days to consider. </param>
        /// <param name="DeviceName"> Device name for the weather data.</param>
        /// <returns> Returns the maximum precipitation DTO. </returns>
        MaxPrecipitationDTO GetMaxPrecipitation(string DeviceName);
        /// <summary>
        /// Retrieves the maximum temperature data between two dates.
        /// </summary>
        /// <param name="start"> Start date of the range. </param>
        /// <param name="finish"> End date of the range. </param>
        /// <returns> Returns an enumerable collection of maximum temperature DTO. </returns>

        List<MaxTempDTO> GetMaxTemperature(DateTime start, DateTime finish);
        /// <summary>
        /// Retrieves filtered weather data based on device name and time.
        /// </summary>
        /// <param name="DeviceName"> Device name from the weather data. </param>
        /// <param name="Time"> The time to filter data. </param>
        /// <returns> Returns an enumerable collection of filtered value DTOs. </returns>
        FilteredValueDTO GetDataValue(string DeviceName, DateTime Time);
        /// <summary>
        /// Retrieves weather data based on the provided ID.
        /// </summary>
        /// <param name="id"> The unique identifier of the weather data records. </param>
        /// <returns>
        /// Returns the weather data associated with the provided ID.
        /// Otherwise, returns null if there's no data found.
        /// </returns>
        /// <remarks>
        /// This method is used to fetch weather data based on a specified ID and field.
        /// </remarks>
        WeatherData GetDataByFieldAndId(string id);
        /// <summary>
        /// Updates weather data records with the provided data.
        /// </summary>
        /// <param name="data"> The updated weather data. </param>
        /// <returns>
        /// Returns a ResponseDTO indication the result of the update operation.
        /// If the operation was successfull, returns a updated weather data.
        /// </returns>
        /// <remarks>
        /// This method is used to update weather data with the provided data.
        /// </remarks>
        ResponseDTO<WeatherData> UpdatedData(WeatherData data);
        /// <summary>
        /// Updates a single weather data entry with the provided updated data list
        /// </summary>
        /// <param name="updatedDataList"> The updated weather data list. </param>
        /// <returns>
        /// Returns a ResponseDTO indicating the result of the update operation.
        /// If the update operation is successful, returns a updated weather data.
        /// </returns>
        /// <remarks>
        /// This method is used to update a single weather data entry with the provided updated data list.
        /// </remarks>
        ResponseDTO<WeatherData> UpdateOne(WeatherData updatedDataList);
        /// <summary>
        /// Retrieves the weather data based on creation dates between two specified dates.
        /// </summary>
        /// <param name="firstCreated"> First created date on the date range given. </param>
        /// <param name="lastCreated"> Last created date on the date range given. </param>
        /// <returns>
        /// Returns a list of weather data witin the specified date range.
        /// </returns>
        List<WeatherData> GetDataByFieldAndDate(DateTime firstCreated, DateTime lastCreated);
        /// <summary>
        /// Updates a single weather data entry.
        /// </summary>
        /// <param name="weatherData"> The weather data to be update.</param>
        /// <returns>
        /// Returns a response that contains information about the update operation.
        /// </returns>
        ResponseDTO<WeatherData> UpdateValue(WeatherData weatherData);
        /// <summary>
        /// Updates multiple weather data records.
        /// </summary>
        /// <param name="updatedDataList"> The list of weather data to be update. </param>
        /// <returns>
        /// Returns a response that contains information about the update operation.
        /// </returns>
        ResponseDTO<WeatherData> UpdateMany(WeatherData updatedDataList);

        List<WeatherData> GetDeletedDataByDate (DateTime firstCreated, DateTime lastCreated, string DeviceName);
        ResponseDTO<WeatherData> UpdateDeletedData (WeatherDataDTO deletedDataList);
        ResponseDTO<WeatherData> DeleteMany(WeatherData deletedData);
        List<WeatherData> GetAll(string DeviceName);
    }
}