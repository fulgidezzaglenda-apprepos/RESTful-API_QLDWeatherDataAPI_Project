using Amazon.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using QLDEducationalWeatherDataAPI.Models.WeatherDTOs;
using QLDEducationalWeatherDataAPI.Services;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace QLDEducationalWeatherDataAPI.Models.Repositories
{
    /// <summary>
    /// Represents the repository for managing the weather data.
    /// </summary>
    public class WeatherRepository : IWeatherRepository
    {
        /// <summary>
        /// Collection for weather data stored in MongoDB.
        /// </summary>
        private readonly IMongoCollection<WeatherData> _dataList;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherRepository"/> class.
        /// </summary>
        /// <param name="connection"> MongoDB connection builder. </param>
        public WeatherRepository(MongoConnectionBuilder connection)
        {
            _dataList = connection.GetDatabase().GetCollection<WeatherData>("WeatherDataSystem");
        }

        /// <summary>
        /// Creates a new single weather data records.
        /// </summary>
        /// <param name="dataList"> This is the weather data to be created. </param>
        public void Create(WeatherData dataList)
        {
            _dataList.InsertOne(dataList);
        }

        /// <summary>
        /// Creates a new multiple weather data records.
        /// </summary>
        /// <param name="dataList"> This is list of weather data to be created. </param>
        public void CreateMany(List<WeatherData> dataList)
        {
            _dataList.InsertMany(dataList);
        }

        /// <summary>
        /// Deletes weather data records by ID.
        /// </summary>
        /// <param name="id"> The unique identifier for the weather data to be deleted. </param>
        /// <returns>
        /// Returns a response indicating the outcome of the delete operation.
        /// </returns>
        public ResponseDTO<WeatherData> Delete(string id)
        {
            ObjectId ObjId = ObjectId.Parse(id);
            var filter = Builders<WeatherData>.Filter.Eq(d => d._id, ObjId);
            var result = _dataList.DeleteOne(filter);

            if (result.DeletedCount > 0)
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "Weather data has been deleted successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                };
            }
            else
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "No weather data has been deleted. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        /// <summary>
        /// Retrieves all weather data records.
        /// </summary>
        /// <returns> Returns an enumerable collection of all weather data records. </returns>
        public IEnumerable<WeatherData> GetAll()
        {
            var builder = Builders<WeatherData>.Filter;
            var filter = builder.Empty;
            return _dataList.Find(filter).ToEnumerable();
        }

        public List<WeatherData> GetAll(string DeviceName)
        {
            var filter = Builders<WeatherData>.Filter.Eq(d => d.DeviceName, DeviceName);
            return _dataList.Find(filter).ToList();
        }

        /// <summary>
        /// Retrieves a weather data records by its Id.
        /// </summary>
        /// <param name="id"> The unique identifier for a weather data record that will be deleted. </param>
        /// <returns>
        /// Returns a weather data records matching to the provided Id.
        /// Otherwise, returns null if not found.
        /// </returns>
        public WeatherData GetById(string id)
        {
            ObjectId ObjId = ObjectId.Parse(id);
            var filter = Builders<WeatherData>.Filter.Eq(d => d._id, ObjId);
            return _dataList.Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a filtered weather data values for a specific device name and time range.
        /// </summary>
        /// <param name="DeviceName"> Device name for weather data reords that needs to be retrieve.</param>
        /// <param name="Time"> The time give by the time range. </param>
        /// <returns>
        /// Returns a collection of filtered weather data values for the specified device name and time range.
        /// </returns>
        public FilteredValueDTO GetDataValue(string DeviceName, DateTime Time)
        {
            var sensorCollection = _dataList.AsQueryable();
            var linqResult = sensorCollection.Where(v => v.Time == Time
                                              && v.DeviceName.Contains(DeviceName))
                                             .Select(v => new FilteredValueDTO
                                             {
                                                 DeviceName = v.DeviceName,
                                                 Time = v.Time,
                                                 AtmosphericPressure = v.AtmosphericPressure,
                                                 Temperature = v.Temperature,
                                                 SolarRadiation = v.SolarRadiation,
                                                 Precipitation = v.Precipitation
                                             }).FirstOrDefault();
            return linqResult;
        }

        /// <summary>
        /// Retrieves the maximum precipitation recorded for a specified number of days and device name.
        /// </summary>
        /// <param name="days"> The number of days from which to retrieve the data.</param>
        /// <param name="DeviceName"> Device name for weather data that needs to be retrieve. </param>
        /// <returns>
        /// Returns the maximum precipitation recorded within the specified time range and device name.
        /// </returns>
        public MaxPrecipitationDTO GetMaxPrecipitation( string DeviceName)
        {
            var sensorCollection = _dataList.AsQueryable();
            var linqResult = sensorCollection.Where(p => p.Time >= DateTime.Now.AddDays(-1400) && p.DeviceName == DeviceName)
                                             .Select(p => new MaxPrecipitationDTO
                                             {
                                                 DeviceName = p.DeviceName,
                                                 Time = p.Time,
                                                 Precipitation = p.Precipitation
                                             }).OrderByDescending(p => p.Precipitation).FirstOrDefault();
            return linqResult;
        }

        /// <summary>
        /// Retrieves the maximum temperature recorded within a specified given time range.
        /// </summary>
        /// <param name="start"> Start time when the given time range. </param>
        /// <param name="finish"> End time when the given time range. </param>
        /// <returns>
        /// Returns a collection of <see cref="MaxTempDTO"/> objects representing the maximum temperature recorded withing the specified time range. 
        /// </returns>
        public List<MaxTempDTO> GetMaxTemperature(DateTime start, DateTime finish)
        {
            // Assuming _dataList is your collection of sensor data
            var sensorCollection = _dataList.AsQueryable();

            // Group by DeviceName and find maximum temperature for each device
            var maxTemperatures = sensorCollection.Where(t => t.Time >= start && t.Time <= finish.AddHours(1))
                                                  .GroupBy(p => p.DeviceName)
                                                  .Select(g => new MaxTempDTO
                                                  {
                                                     DeviceName = g.Key,
                                                     Time = g.OrderByDescending(t => t.Time).FirstOrDefault().Time,
                                                     Temperature = g.Max(t => t.Temperature)
                                                  }).ToList();


            return maxTemperatures;
        }


        /// <summary>
        /// Updates the precipitation data for a specific weather record.
        /// </summary>
        /// <param name="id"> The unique identifier of the weather data record to be update. </param>
        /// <param name="DeviceName"> Device Name match with the weather data records. </param>
        /// <param name="updateData"> Updated weather data that contains the new precipitation value. </param>
        /// <returns>
        /// Returns a <see cref="ResponseDTO{T}"/> indicating the outcoome of the update operation.
        /// </returns>
        public ResponseDTO<WeatherData> Update(string id, string DeviceName, WeatherData updateData)
        {
            ObjectId ObjId = ObjectId.Parse(id);
            var filter = Builders<WeatherData>.Filter.Eq(d => d._id, ObjId);
            var builder = Builders<WeatherData>.Update;
            var update = builder.Set(d => d.Precipitation, updateData.Precipitation);
            var result = _dataList.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "Weather data has been updated successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.ModifiedCount)
                };
            }
            else
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "No weather data has been updated. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        /// <summary>
        /// Updates multiple fields in weather data record.
        /// </summary>
        /// <param name="updatedDataList"> The DTO that contains the updated weather data. </param>
        /// <returns>
        /// Returns a <see cref="ResponseDTO{T}"/> indicating the outcome of the update operation. 
        /// </returns>
        public ResponseDTO<WeatherData> UpdateMany(WeatherData updatedDataList)
        {
            DateTime currentDate = DateTime.UtcNow;
            ObjectId ObjId = ObjectId.Parse(updatedDataList.ObjId);
            var filter = Builders<WeatherData>.Filter.Eq(d => d.ObjId, updatedDataList.ObjId);
            var builder = Builders<WeatherData>.Update;
            var update = builder.Set(d => d.DeviceName, updatedDataList.DeviceName)
                                .Set(d => d.Time, updatedDataList.Time)
                                .Set(d => d.Precipitation, updatedDataList.Precipitation)
                                .Set(d => d.Latitude, updatedDataList.Latitude)
                                .Set(d => d.Longitude, updatedDataList.Longitude)
                                .Set(d => d.Temperature, updatedDataList.Temperature)
                                .Set(d => d.AtmosphericPressure, updatedDataList.AtmosphericPressure)
                                .Set(d => d.WindSpeed, updatedDataList.WindSpeed)
                                .Set(d => d.SolarRadiation, updatedDataList.SolarRadiation)
                                .Set(d => d.VaporPressure, updatedDataList.VaporPressure)
                                .Set(d => d.Humidity, updatedDataList.Humidity)
                                .Set(d => d.WindDirection, updatedDataList.WindDirection);

            var result = _dataList.UpdateOne(filter, update);
            if (result.ModifiedCount > 0)
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "Weather data has been updated successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.MatchedCount)
                };
            }
            else
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "No weather data has been updated. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        /// <summary>
        /// Retrieves weather data records within a specified date range.
        /// </summary>
        /// <param name="firstCreated"> First created date use to filtered weather data by date range. </param>
        /// <param name="lastCreated"> Last created date use to filtered weather data by date range. </param>
        /// <returns>
        /// Returns a list of weather data within the specified date range.
        /// </returns>
        public List<WeatherData> GetDataByFieldAndDate(DateTime firstCreated, DateTime lastCreated)
        {
            var filter = Builders<WeatherData>.Filter.And(
                         Builders<WeatherData>.Filter.Gte(d => d.Created, firstCreated),
                         Builders<WeatherData>.Filter.Lte(d => d.Created, lastCreated));
            return _dataList.Find(filter).ToList();
        }

        /// <summary>
        /// Updates values of weather data records.
        /// </summary>
        /// <param name="weatherData"> The weather data with updated values. </param>
        /// <returns>
        /// Returns a <see cref="ResponseDTO{T}"/> indicating the outcome of the update operation.
        /// </returns>
        public ResponseDTO<WeatherData> UpdateValue(WeatherData weatherData)
        {
            ObjectId ObjId = ObjectId.Parse(weatherData.ObjId);
            var filter = Builders<WeatherData>.Filter.Eq(u => u._id, ObjId);
            var builder = Builders<WeatherData>.Update;
            var update = builder.Set(d => d.Precipitation, weatherData.Precipitation)
                                .Set(d => d.Latitude, weatherData.Latitude)
                                .Set(d => d.Longitude, weatherData.Longitude)
                                .Set(d => d.Temperature, weatherData.Temperature)
                                .Set(d => d.AtmosphericPressure, weatherData.AtmosphericPressure)
                                .Set(d => d.WindSpeed, weatherData.WindSpeed)
                                .Set(d => d.SolarRadiation, weatherData.SolarRadiation)
                                .Set(d => d.VaporPressure, weatherData .VaporPressure)
                                .Set(d => d.Humidity, weatherData.Humidity)
                                .Set(d => d.WindDirection, weatherData.WindDirection);
            var result = _dataList.UpdateOne(filter, update);
            if (result.ModifiedCount > 0)
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "New data value has been updated successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.MatchedCount)
                };
            }
            else
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "No new data value has been updated. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

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
        public WeatherData GetDataByFieldAndId(string id)
        {
            ObjectId ObjId = ObjectId.Parse(id);
            var filter = Builders<WeatherData>.Filter.Eq(d => d._id, ObjId);
            return _dataList.Find(filter).FirstOrDefault();
        }

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
        public ResponseDTO<WeatherData> UpdatedData(WeatherData data)
        {
            ObjectId ObjId = ObjectId.Parse(data.ObjId);
            var filter = Builders<WeatherData>.Filter.Eq(u => u._id, ObjId);
            var builder = Builders<WeatherData>.Update;
            var update = builder.Set(d => d.Precipitation, data.Precipitation)
                                .Set(d => d.Latitude, data.Latitude)
                                .Set(d => d.Longitude, data.Longitude)
                                .Set(d => d.Temperature, data.Temperature)
                                .Set(d => d.AtmosphericPressure, data.AtmosphericPressure)
                                .Set(d => d.WindSpeed, data.WindSpeed)
                                .Set(d => d.SolarRadiation, data.SolarRadiation)
                                .Set(d => d.VaporPressure, data.VaporPressure)
                                .Set(d => d.Humidity, data.Humidity)
                                .Set(d => d.WindDirection, data.WindDirection);
            var result = _dataList.UpdateOne(filter, update);
            if (result.ModifiedCount > 0)
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "New data value has been updated successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.MatchedCount)
                };
            }
            else
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "No new data value has been updated. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        //// <summary>
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
        public ResponseDTO<WeatherData> UpdateOne(WeatherData updatedDataList)
        {
            DateTime currentDate = DateTime.UtcNow;
            ObjectId ObjId = ObjectId.Parse(updatedDataList.ObjId);
            var filter = Builders<WeatherData>.Filter.Eq(d => d.ObjId, updatedDataList.ObjId);
            var builder = Builders<WeatherData>.Update;
            var update = builder.Set(d => d.DeviceName, updatedDataList.DeviceName)
                                .Set(d => d.Time, updatedDataList.Time)
                                .Set(d => d.Precipitation, updatedDataList.Precipitation)
                                .Set(d => d.Latitude, updatedDataList.Latitude)
                                .Set(d => d.Longitude, updatedDataList.Longitude)
                                .Set(d => d.Temperature, updatedDataList.Temperature)
                                .Set(d => d.AtmosphericPressure, updatedDataList.AtmosphericPressure)
                                .Set(d => d.WindSpeed, updatedDataList.WindSpeed)
                                .Set(d => d.SolarRadiation, updatedDataList.SolarRadiation)
                                .Set(d => d.VaporPressure, updatedDataList.VaporPressure)
                                .Set(d => d.Humidity, updatedDataList.Humidity)
                                .Set(d => d.WindDirection, updatedDataList.WindDirection);

            var result = _dataList.UpdateOne(filter, update);
            if (result.ModifiedCount > 0)
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "Weather data has been updated successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.MatchedCount)
                };
            }
            else
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "No weather data has been updated. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        public List<WeatherData> GetDeletedDataByDate(DateTime firstCreated, DateTime lastCreated, string DeviceName)
        {
            var filter = Builders<WeatherData>.Filter.And(
                         Builders<WeatherData>.Filter.Eq(d => d.DeviceName, DeviceName),
                         Builders<WeatherData>.Filter.Gte(d => d.Created, firstCreated),
                         Builders<WeatherData>.Filter.Lte(u => u.Created, lastCreated));
            return _dataList.Find(filter).ToList();
        }

        public ResponseDTO<WeatherData> UpdateDeletedData(WeatherDataDTO deletedDataList)
        {
            ObjectId Objid = ObjectId.Parse(deletedDataList.ObjId);
            var filter = Builders<WeatherData>.Filter.And(
                         Builders<WeatherData>.Filter.Eq(d => d._id, Objid),
                         Builders<WeatherData>.Filter.Eq(d => d.DeviceName, deletedDataList.DeviceName));
            var result = _dataList.DeleteOne(filter);

            if (result.DeletedCount > 0)
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "Weather data records deleted successfully!",
                    WasSuccessful = true,
                    RecordsAffected = Convert.ToInt32(result.DeletedCount)
                };
            }
            else
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "No records has been deleted. Please check details and try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }

        public ResponseDTO<WeatherData> DeleteMany(WeatherData deletedData)
        {
            try
            {
                DateTime firstCreated = DateTime.UtcNow;
                DateTime lastCreated = DateTime.UtcNow;
                var filter = Builders<WeatherData>.Filter.Eq(d => d.DeviceName, deletedData.DeviceName);
                Builders<WeatherData>.Update.Set(d => d.DeviceName, deletedData.DeviceName);
                Builders<WeatherData>.Update.Set(d => d.Created, deletedData.Created);
                var result = _dataList.DeleteMany(filter);
                if (result.DeletedCount > 0)
                {
                    return new ResponseDTO<WeatherData>
                    {
                        Message = "Weather data records has been successfully deleted!",
                        WasSuccessful = true,
                        RecordsAffected = Convert.ToInt32(result.DeletedCount)
                    };
                }
                else
                {
                    return new ResponseDTO<WeatherData>
                    {
                        Message = "No weather data records matching the criteria have been found or deleted!",
                        WasSuccessful = false,
                        RecordsAffected = 0
                    };
                }
            }
            catch (Exception)
            {
                return new ResponseDTO<WeatherData>
                {
                    Message = "An error occurred while deleting user/s account. Please check all details are correct then try again!",
                    WasSuccessful = false,
                    RecordsAffected = 0
                };
            }
        }
    }
}
