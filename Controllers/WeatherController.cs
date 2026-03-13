using Amazon.Runtime.SharedInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using QLDEducationalWeatherDataAPI.AttributeTags;
using QLDEducationalWeatherDataAPI.Models;
using QLDEducationalWeatherDataAPI.Models.Repositories;
using QLDEducationalWeatherDataAPI.Models.UserDTOs;
using QLDEducationalWeatherDataAPI.Models.WeatherDTOs;
using System.Data;
using ZstdSharp.Unsafe;

namespace QLDEducationalWeatherDataAPI.Controllers
{
    /// <summary>
    /// Controller for managing the weather data related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        /// <summary>
        /// Represents the weather repository used for accessing weather data.
        /// </summary>
        private readonly IWeatherRepository _weatherRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherController"/> class.
        /// </summary>
        /// <param name="weatherRepository"> The weather repository used for accessing weather data. </param>
        public WeatherController(IWeatherRepository weatherRepository)
        {
            // Assigning the provided weather repository to the local variable.
            _weatherRepository = weatherRepository;
        }

        /// <summary>
        /// This method is a delete operation that delete weather data based on the provided identifier "ID".
        /// </summary>
        /// <param name="id"> This is the identifier of the weather data tthat need to be deleted. </param>
        /// <returns>
        /// Returns a BadRequest response if the'ID' is null, empty, or whitespace, indicating a valid '_id' is required.
        /// Otherwise, returns OK response with the result of the deletion operation.
        /// </returns>
        //DELETE: api/<WeatherController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult Delete(string id)
        {
            // Checking if the 'ID' parameter is null, empty, or contains only whitespace characters.
            if (String.IsNullOrWhiteSpace(id))
            {
                // Returning a BadRequest response with a message indicating that a valid '_id' is required for the operation.
                return BadRequest("A valid _id is required to perform this operation!");
            }

            // Calling the Delete method of _weatherRepository responsibly for managing weather data,
            // and then passing the 'id' parameter storing the result of the delete operation in the 'result' variable.
            var result = _weatherRepository.Delete(id);
            // Checking if the deletion operation was not successful.
            if (result.WasSuccessful == false)
            {
                // Then return a BadRequest response with the 'result' object if its null.
                return BadRequest(result);
            }
            // Then returning an OK response with the 'result' object indicating that the delete operation was successful.
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstCreated"></param>
        /// <param name="lastCreated"></param>
        /// <param name="DeviceName"></param>
        /// <returns></returns>
        //DELETE: api/<WeatherController> DELETEMANY
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("DeleteMany")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult DeleteMany (DateTime firstCreated, DateTime lastCreated, string DeviceName)
        {
            
            if (String.IsNullOrWhiteSpace(DeviceName))
            {
                return BadRequest($"A valid role is required to perform this operation!:");
            }
            if (firstCreated == default(DateTime) || lastCreated == default(DateTime))
            {
                return BadRequest($"A valid date is required to perform this operation: {firstCreated.Date} and {lastCreated.Date}");
            }

            List<WeatherDataDTO> updateDeletedData = new List<WeatherDataDTO>();
            var datasToDelete = _weatherRepository.GetDeletedDataByDate(firstCreated, lastCreated, DeviceName);
            if (datasToDelete == null)
            {
                return BadRequest($"No weather date records found with Device Name '{DeviceName}' between date created '{firstCreated}' and '{lastCreated}'.");
            }

            foreach (var weatherDTO in datasToDelete)
            {
                var deletedData = new WeatherDataDTO
                {
                    _id = weatherDTO._id,
                    DeviceName = DeviceName,
                    FirstCreated = weatherDTO.Created,
                    LastCreated = weatherDTO.Created,
                };
                var result = _weatherRepository.UpdateDeletedData(deletedData);
                // Checks if the update operation was not successful.
                if (!result.WasSuccessful)
                {
                    // If its not, returns a BadRequest response indicating failed to delete user accounts.
                    return BadRequest($"Failed to delete users!");
                }
                // Adding all deleted user accounts details to the list.
                updateDeletedData.Add(deletedData);
            }
            // Returns an OK response indicating successful deletion of user accounts.
            return Ok($"Users account have been successfully deleted! Total users account deleted: {updateDeletedData.Count}");
        }

        /// <summary>
        /// This retrieves all weather data in our database.
        /// </summary>
        /// <returns> Return OK response with a list of weather data if the operation is successful. </returns>
        //GET: api/WeatherController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [ApiKey(UserRoles.TEACHER,  UserRoles.STUDENT)]
        public  ActionResult <List<WeatherData>> Get(string DeviceName)
        {
            // This retrieve all the weather data from our repository.
            var weatherData = _weatherRepository.GetAll(DeviceName);
            if (string.IsNullOrEmpty(DeviceName))
            {
                return BadRequest("Device Name is not a valid entry");
            }
            
            // Then return an OK response with the retrieved weather data from our database.
            return weatherData;
        }

        /// <summary>
        /// This retrieve all filtered weather data values based on the specified device name and time range.
        /// </summary>
        /// <param name="DeviceName"> The name of the device for which data is to be retrieve. </param>
        /// <param name="Time"> The specified time for which data is to be retrieve. </param>
        /// <returns> Returns an enumerable collection of <see cref="FilteredValueDTO"/> that contains filtered weather data values. </returns>
        //GET: api/<WeatherController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("GetDataValue")]
        [ApiKey(UserRoles.TEACHER, UserRoles.STUDENT)]
        public FilteredValueDTO GetDataValue(string DeviceName, DateTime Time)
        {
            // Retrieve filtered weather data values from our repository based on the specified parameters.
            var result = _weatherRepository.GetDataValue(DeviceName, Time);
            // Then returns the retrieved filtered weather data values.
            return result;
        }

        /// <summary>
        /// Retrieves the maximum value of Precipitation data withing a specified number of days for the specific Device with DeviceName.
        /// </summary>
        /// <param name="days"> Numbers of days to consider for calculating maximum precipitation. </param>
        /// <param name="DeviceName"> Name of the device for which maximum precipitation data is to be retrieved. </param>
        /// <returns> Returns a <see cref="MaxPrecipitationDTO"/> object that contains information about the maximum precipitation within the specified days.
        /// </returns>
        //GET: api/<WeatherController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("GetMaxPrecipitation")]
        [ApiKey(UserRoles.TEACHER, UserRoles.STUDENT)]
        public MaxPrecipitationDTO GetMaxPrecipitation( string DeviceName)
        {
            // Retrieves the maximum preecipitation data within the specified number of days given for
            // the device from our repository.
            var result = _weatherRepository.GetMaxPrecipitation(DeviceName);
            // Returns the maximum precipitation data.
            return result;
        }

        /// <summary>
        /// Retrieves maximum temperature data within the specified time range.
        /// </summary>
        /// <param name="start"> The start date and time of the time range. </param>
        /// <param name="finish"> The end fate and time of the time range. </param>
        /// <returns> Returns an enumerable collection of <see cref="MaxTempDTO"/> object that contains maximum temperature data. </returns>
        //GET: api/<WeatherController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("GetMaxTemperature")]
        [ApiKey(UserRoles.TEACHER, UserRoles.STUDENT)]
        public List<MaxTempDTO> GetMaxTemperature(DateTime start, DateTime finish)
        {
            // Retrieves maximum temperature data within the specified time range from the our repository.
            var result = _weatherRepository.GetMaxTemperature(start, finish);
            // Then returns the maximum temperature data.
            return result;
        }

        /// <summary>
        /// Updates a single field of weather data records identified by the provided ID.
        /// </summary>
        /// <param name="id"> The unique identifier of the weather data records. </param>
        /// <param name="fieldName"> The name of the field to update. </param>
        /// <param name="newValue"> The new value to set for the specified field. </param>
        /// <returns>
        /// Returns ac ActionResult indicating the result of the operation.
        /// If it is successful, returns an OK response message indicating of the success operation.
        /// Otherwise, returns a BadRequest response indicating the right message depending on the errors.
        /// </returns>
        //PUT: api/<WeatherController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("UpdateOne")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult UpdateOne(string id, string fieldName, double newValue)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                // If its null, returns a BadRequest response with a message indicating that valid parameters are required for this action.
                return BadRequest("A valid id is required to perform this action");
            }

            var weatherDataList = _weatherRepository.GetDataByFieldAndId(id);
            if (weatherDataList == null)
            {
                // If no weather data is found for the provided ID, returns a BadRequest response message indicating
                // a valid Id is requiered to perform this action.
                return BadRequest($"A valid Id: '{id}' is required to perform this operation.");
            }

            var data = weatherDataList; 
            {
                // Then, updates the specified field based on the provided fieldName in the parameter with a given newValue,
                // leaving the other field with the same value.
                if (fieldName == "Precipitation")
                {
                    data.Precipitation = newValue;
                }
                else if (fieldName == "Latitude")
                {
                    data.Latitude = newValue;
                }
                else if (fieldName == "Longitude")
                {
                    data.Longitude = newValue;
                }
                else if (fieldName == "Temperature")
                {
                    data.Temperature = newValue;
                }
                else if (fieldName == "Atmospheric Pressure")
                {
                    data.AtmosphericPressure = newValue;
                }
                else if (fieldName == "WindSpeed")
                {
                    data.WindSpeed = newValue;
                }
                else if (fieldName == "Solar Radiation")
                {
                    data.SolarRadiation = newValue;
                }
                else if (fieldName == "Vapor Pressure")
                {
                    data.VaporPressure = newValue;
                }
                else if (fieldName == "Humidity")
                {
                    data.Humidity = newValue;
                }
                else if (fieldName == "WindDirection")
                {
                    data.WindDirection = newValue;
                }
                else
                {
                    // Returning a BadRequest response with a message indicating an matched fieldName in the weather data.
                    return BadRequest($"There are no fieldName '{fieldName}' matching in the Weather Data.");
                }

                var result = _weatherRepository.UpdatedData(data);
                if (!result.WasSuccessful)
                {
                    // If the update operation fails, returns a BadRequest response indicating the failed to update weather data.
                    return BadRequest($"Failed to update '{fieldName}' in weather data.");
                }
            };
            // If the update operation is successful, returns an OK response with a success message.
            return Ok($"Field '{fieldName}' updated successfully with the value '{newValue}' for Id: '{id}' in weather data.");
        }

        /// <summary>
        /// This method is a update operation that update multiple records within the specified date range with a new value for the specified fieldName.
        /// </summary>
        /// <param name="firstCreated"> This is when the first weather data records created and use as a parameter for a specific date/time range. </param>
        /// <param name="lastCreated"> This is when the last weather data records created and use as a parameter for a specific date/time range.</param>
        /// <param name="fieldName"> The name of the field to update in the weather data records. </param>
        /// <param name="newValue"> The new value given for the specific field in the weather data. </param>
        /// <returns>
        /// Returns an OK response if the operation is successfull, indicating the number of records updated.
        /// Otherwise, returns a BadRequest response if any validation fails or if the update operation encounters errors.
        /// </returns>
        //PACTH: api/<WeatherController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("UpdateMany")]
        [ApiKey(UserRoles.TEACHER)]
        public ActionResult UpdateMany(DateTime firstCreated, DateTime lastCreated, string fieldName, double newValue)
        {
            // Checking if either the 'firstCreated' or 'lastCreated' parameter is the default DateTime value.
            if (firstCreated == default(DateTime) || lastCreated == default(DateTime))
            {
                // If not, returns a BadRequest response with a message indicaating that a valid date are required to
                // performed this operation.
                return BadRequest($"A valid date is required to perform this operation: {firstCreated.Date} and {lastCreated.Date}");
            }

            // This is where weather data records being filtered and retrieve within the specified date range.
            var weatherDataList = _weatherRepository.GetDataByFieldAndDate(firstCreated, lastCreated);
            // Checking if there are no weather data records found withing the specified date range.
            if (weatherDataList == null || weatherDataList.Count == 0)
            {
                // If it is bull, then returns a BadRequest response with a message indicating that there are no weather data
                // found within the specfied date range.
                return BadRequest($"No weather data found between the date created '{firstCreated}' and '{lastCreated}'.");
            }
            
            // This loops iterates through each weather data records in the retrieved list.
            foreach (var weatherData in weatherDataList)
            {
                // Then, updates the specified field based on the provided fieldName in the parameter with a given newValue,
                // leaving the other field with the same value.
                if (fieldName == "Precipitation")
                {
                    weatherData.Precipitation = newValue;
                }
                else if (fieldName == "Latitude")
                {
                    weatherData.Latitude = newValue;
                }
                else if (fieldName == "Longitude")
                {
                    weatherData.Longitude = newValue;
                }
                else if (fieldName == "Temperature")
                {
                    weatherData.Temperature = newValue;
                }
                else if (fieldName == "Atmospheric Pressure")
                {
                    weatherData.AtmosphericPressure = newValue;
                }
                else if (fieldName == "WindSpeed")
                {
                    weatherData.WindSpeed = newValue;
                }
                else if (fieldName == "Solar Radiation")
                {
                    weatherData.SolarRadiation = newValue;
                }
                else if (fieldName == "Vapor Pressure")
                {
                    weatherData.VaporPressure = newValue;
                }
                else if (fieldName == "Humidity")
                {
                    weatherData.Humidity = newValue;
                }
                else if (fieldName == "WindDirection")
                {
                   weatherData.WindDirection = newValue;
                }
                else
                {
                    // Returning a BadRequest response with a message indicating an matched fieldName in the weather data.
                   return BadRequest($"There are no fieldName '{fieldName}' matching in the Weather Data.");
                }
                
                // Then, updates the weather data records in our repository.
                var result = _weatherRepository.UpdateValue(weatherData);
                // Checks if the update operation was not successful.
                if (!result.WasSuccessful)
                {
                    // If not, then returns a BadRequest with a message indicating the failure updating the weather data.
                    return BadRequest($"Failed to update weather data '{fieldName}'.");
                }
            }
            // If it successful, return an OK response indicating the success of the update operation with the specified field for all records.
            return Ok($"Field '{fieldName}' updated successfully with value '{newValue}' for '{weatherDataList.Count}' records in Weather Data");
        }

        /// <summary>
        /// This method is a create operation that create a new single weather data record.
        /// </summary>
        /// <param name="createData"> This is the weatherData to be created. </param>
        /// <returns>
        /// Returns an OK response if the creating is successfull
        /// Returns a BadRequest response if the provided data is null.
        /// Or, returns a problem response with a status code of 500 if an error occurs during the creation process.
        /// </returns>
        //POST: api/<WeatherController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost("Post")]
        [ApiKey(UserRoles.SENSOR, UserRoles.TEACHER)]
        public ActionResult Post([FromBody] WeatherData? createData)
        {
            // Checks if the provided weather data is null.
            if (createData == null)
            {
                // If it is, then returns a BadRequest.
                return BadRequest();
            }
            try
            {
                // Attempts to create the weather data record using our repository.
                _weatherRepository.Create(createData);
                // If its successful, returns an OK response indicating the success of creation.
                return Ok("New Weather data has been successfully created!");
            }
            catch (Exception ex)
            {
                // Returns a Problem response with a Status Code of 500 if an error occur during the creation process.
                return Problem(detail:ex.Message, statusCode : 500);
            }
        }

        /// <summary>
        /// This method is a create operation that create multiple weather data records in a single query.
        /// </summary>
        /// <param name="createData"> This is list of weather data to be created. </param>
        /// <returns>
        /// Returns an OK response if the creation is sucessful.
        /// Otherwise, returns a BadRequest if the provided data is null or empty.
        /// </returns>
        //POST: api/<WeatherController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost("PostMany")]
        [ApiKey(UserRoles.SENSOR, UserRoles.TEACHER)]
        public ActionResult PostMany([FromBody] List<WeatherData>? createData)
        {
            // Checks if the provided list of weather data is null or empty.
            if (createData == null || createData.Count == 0)
            {
                // If it is, returns a BadRequest response.
                return BadRequest();
            }
            // Attempts to create multiple weather data records using our repository.
            _weatherRepository.CreateMany(createData);
            // Then, returns an OK response indicating successful creation of the list of weather data.
            return Ok("New list of weather data has been successfuly created!");
        }
    }
}
